using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace Blood_Bank.Controllers
{
    [Authorize( Roles = "Hospital" )]
    public class HospitalController : Controller
    {
        private readonly IBloodTestService _bloodTestService;
        private readonly IDonationService _donationService;
        private readonly IBloodRequestService _bloodRequestService;
        private readonly UserManager<User> _userManager;

        public HospitalController (
            IBloodTestService bloodTestService,
            IDonationService donationService,
            IBloodRequestService bloodRequestService,
            UserManager<User> userManager )
        {
            _bloodTestService = bloodTestService;
            _donationService = donationService;
            _bloodRequestService = bloodRequestService;
            _userManager = userManager;
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

        // ----- Blood Request Management -----
        // GET: Hospital/CreateBloodRequest
        public IActionResult CreateBloodRequest ()
        {
            return View();
        }

        // POST: Hospital/CreateBloodRequest
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBloodRequest ( CreateBloodRequestDto dto )
        {
            if ( !ModelState.IsValid )
                return View( dto );

            // Set the hospital ID to the currently logged-in hospital.
            // Adjust the parsing if your HospitalId is string.
            dto.HospitalId = int.Parse( _userManager.GetUserId( User ) );
            await _bloodRequestService.CreateBloodRequestAsync( dto );
            TempData [ "Success" ] = "Blood request created successfully.";
            return RedirectToAction( nameof( BloodRequests ) );
        }

        // GET: Hospital/BloodRequests
        public async Task<IActionResult> BloodRequests ()
        {
            var requests = await _bloodRequestService.GetAllBloodRequestsAsync();
            var currentHospitalId = _userManager.GetUserId( User );
            var filteredRequests = requests.Where( r => r.HospitalId.ToString() == currentHospitalId );
            return View( filteredRequests );
        }
    }
}
