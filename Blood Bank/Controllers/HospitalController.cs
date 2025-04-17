using BloodBank.Application.Services;
using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace BloodBank.Web.Controllers // Aligned namespace with previous code
{
    [Authorize( Roles = "Hospital" )]
    public class HospitalController : Controller
    {
        private readonly IBloodTestService _bloodTestService;
        private readonly IDonationService _donationService;
        private readonly IBloodUnitService bloodUnitService;
        private readonly UserManager<User> _userManager;

        public HospitalController (
            IBloodTestService bloodTestService,
            IDonationService donationService,
            IBloodUnitService bloodUnitService,
            UserManager<User> userManager )
        {
            _bloodTestService = bloodTestService;
            _donationService = donationService;
            _userManager = userManager;
            this.bloodUnitService = bloodUnitService;
        }

        // ----- Blood Test Approval -----
        // GET: Hospital/BloodTestsPending
        public async Task<IActionResult> BloodTestsPending ()
        {
            var currentHospitalId = _userManager.GetUserId( User );
            var pendingTests = await _bloodTestService.GetPendingTestsAsync();
            var filteredTests = pendingTests.Where( t => t.HospitalId == currentHospitalId );
            return View( filteredTests );
        }

        // POST: Hospital/ApproveBloodTest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveBloodTest ( int id )
        {
            var currentHospitalId = _userManager.GetUserId( User );
            var test = await _bloodTestService.GetTestByIdAsync( id );
            if ( test == null )
            {
                TempData [ "Error" ] = "Blood test not found.";
                return RedirectToAction( nameof( BloodTestsPending ) );
            }
            if ( test.HospitalId != currentHospitalId )
            {
                TempData [ "Error" ] = "You are not authorized to approve this blood test.";
                return RedirectToAction( nameof( BloodTestsPending ) );
            }
            await _bloodTestService.ApproveBloodTestAsync( id );
            TempData [ "Success" ] = "Blood test approved successfully.";
            return RedirectToAction( nameof( BloodTestsPending ) );
        }

        // POST: Hospital/RejectBloodTest/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectBloodTest ( int id )
        {
            var currentHospitalId = _userManager.GetUserId( User );
            var test = await _bloodTestService.GetTestByIdAsync( id );
            if ( test == null )
            {
                TempData [ "Error" ] = "Blood test not found.";
                return RedirectToAction( nameof( BloodTestsPending ) );
            }
            if ( test.HospitalId != currentHospitalId )
            {
                TempData [ "Error" ] = "You are not authorized to reject this blood test.";
                return RedirectToAction( nameof( BloodTestsPending ) );
            }
            await _bloodTestService.RejectBloodTestAsync( id );
            TempData [ "Success" ] = "Blood test rejected successfully.";
            return RedirectToAction( nameof( BloodTestsPending ) );
        }

        // ----- Donation Approval -----
        // GET: Hospital/PendingDonations
        public async Task<IActionResult> PendingDonations ()
        {
            var currentHospitalId = _userManager.GetUserId( User );
            var donations = await _donationService.GetDonationsByHospitalAsync( currentHospitalId );
            var pendingDonations = donations.Where( d => d.Status == DonationStatus.Pending );
            return View( pendingDonations );
        }

        // POST: Hospital/ApproveDonation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveDonation ( int id )
        {
            var currentHospitalId = _userManager.GetUserId( User );
            var hospital = await _userManager.FindByIdAsync( currentHospitalId );
            var donation = await _donationService.GetDonationByIdAsync( id );
            if ( donation == null )
            {
                TempData [ "Error" ] = "Donation not found.";
                return RedirectToAction( nameof( PendingDonations ) );
            }
            if ( donation.HospitalId != currentHospitalId )
            {
                TempData [ "Error" ] = "You are not authorized to approve this donation.";
                return RedirectToAction( nameof( PendingDonations ) );
            }
            await _donationService.UpdateDonationStatusAsync( id, DonationStatus.Approved );
            await bloodUnitService.CreateBloodUnitAsync( new CreateBloodUnitDto
            {
                DonationId = id,
                BloodType = donation.BloodType,
                Quantity = donation.Quantity,
                StorageLocation = hospital.FirstName + " " + hospital.LastName,
            } );
            TempData [ "Success" ] = "Donation approved successfully.";
            return RedirectToAction( nameof( PendingDonations ) );
        }

        // POST: Hospital/RejectDonation/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectDonation ( int id )
        {
            var currentHospitalId = _userManager.GetUserId( User );
            var donation = await _donationService.GetDonationByIdAsync( id );
            if ( donation == null )
            {
                TempData [ "Error" ] = "Donation not found.";
                return RedirectToAction( nameof( PendingDonations ) );
            }
            if ( donation.HospitalId != currentHospitalId )
            {
                TempData [ "Error" ] = "You are not authorized to reject this donation.";
                return RedirectToAction( nameof( PendingDonations ) );
            }
            await _donationService.UpdateDonationStatusAsync( id, DonationStatus.Rejected );
            TempData [ "Success" ] = "Donation rejected successfully.";
            return RedirectToAction( nameof( PendingDonations ) );
        }
    }
}