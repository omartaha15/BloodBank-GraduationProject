using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blood_Bank.Controllers
{
    [Authorize]
    public class DonationController : Controller
    {
        private readonly IDonationService _donationService;
        private readonly UserManager<User> _userManager; // Add this

        public DonationController (
            IDonationService donationService,
            UserManager<User> userManager ) // Add UserManager
        {
            _donationService = donationService;
            _userManager = userManager;
        }

        // GET: Donation
        public async Task<IActionResult> Index ()
        {
            var donations = await _donationService.GetAllDonationsAsync();
            return View( donations );
        }

        public IActionResult Create ()
        {
            var createDto = new CreateDonationDto
            {
                DonorId = _userManager.GetUserId( User )  // Set DonorId here
            };
            return View( createDto );
        }

        // POST: Donation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create ( CreateDonationDto donationDto )
        {
            if ( !ModelState.IsValid )
                return View( donationDto );

            try
            {
                // No need to set DonorId here anymore as it's already in the form
                await _donationService.CreateDonationAsync( donationDto );
                TempData [ "Success" ] = "Donation recorded successfully.";
                return RedirectToAction( nameof( Index ) );
            }
            catch ( Exception ex )
            {
                ModelState.AddModelError( "", ex.Message );
                return View( donationDto );
            }
        }

        // GET: Donation/Details/5
        public async Task<IActionResult> Details ( int id )
        {
            var donation = await _donationService.GetDonationByIdAsync( id );
            if ( donation == null )
                return NotFound();

            return View( donation );
        }
    }
}
