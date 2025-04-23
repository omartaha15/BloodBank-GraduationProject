using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using BloodBank.Core.Entities.BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blood_Bank.Controllers
{
    [Authorize]
    public class DonationController : Controller
    {
        private readonly IDonationService _donationService;
        private readonly IBloodTestService _bloodTestService;
        private readonly UserManager<User> _userManager;

        public DonationController (
            IDonationService donationService,
            IBloodTestService bloodTestService,
            UserManager<User> userManager )
        {
            _donationService = donationService;
            _bloodTestService = bloodTestService;
            _userManager = userManager;
        }

        // GET: Donation
        public async Task<IActionResult> Index ()
        {
            var userId = User.FindFirst( ClaimTypes.NameIdentifier )?.Value;
            var donations = await _donationService.GetDonationsByDonorAsync( userId );

            var lastDonation = donations.OrderByDescending( d => d.DonationDate ).FirstOrDefault();
            if ( lastDonation != null )
            {
                var daysSinceLastDonation = ( DateTime.Now - lastDonation.DonationDate ).Days;
                ViewBag.DaysSinceLastDonation = daysSinceLastDonation;
            }
            else
            {
                ViewBag.DaysSinceLastDonation = 999; // Allow donations if no previous record
            }

            return View( donations );
        }



        public async Task<IActionResult> Details (int id)
        {
            var _donation = await _donationService.GetDonationByIdAsync( id );
            return View( _donation );
        }

        // GET: Donation/Create
        public async Task<IActionResult> Create ()
        {
            var donorId = _userManager.GetUserId( User );

            // Check if the donor has a valid blood test.
            try
            {
                var bloodTest = await _bloodTestService.GetTestByDonorIdAsync( donorId );
                // If blood test is null, not passed, or not approved, redirect.
                if ( bloodTest == null || !bloodTest.IsTestPassed || bloodTest.HospitalApprovalStatus != HospitalApprovalStatus.Approved )
                {
                    TempData [ "Error" ] = "You must complete a valid blood test (passed and approved by a hospital) before making a donation.";
                    return RedirectToAction( "Create", "BloodTest" );
                }
            }
            catch ( NotFoundException )
            {
                TempData [ "Error" ] = "You must complete a blood test before making a donation.";
                return RedirectToAction( "Create", "BloodTest" );
            }

            // Retrieve the list of hospitals (users with the Hospital role) for the dropdown.
            var hospitals = await _userManager.GetUsersInRoleAsync( "Hospital" );
            ViewBag.Hospitals = hospitals
                .Select( h => new { h.Id, FullName = $"{h.FirstName} {h.LastName}" } )
                .ToList();

            // Prepare the donation creation DTO.
            var createDto = new CreateDonationDto
            {
                DonorId = donorId,
                AppointmentDate = DateTime.Today.AddDays( 1 ),
                AppointmentTime = new TimeSpan( 9, 0, 0 ),
                AppointmentNotes = string.Empty
            };

            return View( createDto );
        }


        // POST: Donation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create ( CreateDonationDto donationDto )
        {
            if ( !ModelState.IsValid )
            {
                var hospitals = await _userManager.GetUsersInRoleAsync( "Hospital" );
                ViewBag.Hospitals = hospitals
                    .Select( h => new { h.Id, FullName = $"{h.FirstName} {h.LastName}" } )
                    .ToList();
                return View( donationDto );
            }

            try
            {
                await _donationService.CreateDonationAsync( donationDto );
                TempData [ "Success" ] = "Donation recorded successfully.";
                return RedirectToAction( nameof( Index ) );
            }
            catch ( InvalidOperationException ex )
            {
                // This might occur if the donor is trying to donate before 3 months
                // or if their blood test hasn't been completed/approved.
                ModelState.AddModelError( "", ex.Message );
                var hospitals = await _userManager.GetUsersInRoleAsync( "Hospital" );
                ViewBag.Hospitals = hospitals
                    .Select( h => new { h.Id, FullName = $"{h.FirstName} {h.LastName}" } )
                    .ToList();
                return View( donationDto );
            }
            catch ( Exception ex )
            {
                ModelState.AddModelError( "", ex.Message );
                var hospitals = await _userManager.GetUsersInRoleAsync( "Hospital" );
                ViewBag.Hospitals = hospitals
                    .Select( h => new { h.Id, FullName = $"{h.FirstName} {h.LastName}" } )
                    .ToList();
                return View( donationDto );
            }
        }
    }
}
