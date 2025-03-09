using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using System.Linq;
using System.Threading.Tasks;

namespace Blood_Bank.Controllers
{
    [Authorize( Roles = "Donor" )]
    public class BloodTestController : Controller
    {
        private readonly IBloodTestService _bloodTestService;
        private readonly UserManager<User> _userManager;

        public BloodTestController ( IBloodTestService bloodTestService, UserManager<User> userManager )
        {
            _bloodTestService = bloodTestService;
            _userManager = userManager;
        }

        // GET: BloodTest/Create
        public async Task<IActionResult> Create ()
        {
            var donorId = _userManager.GetUserId( User );

            // Check if the donor already has a blood test.
            try
            {
                var existingTest = await _bloodTestService.GetTestByDonorIdAsync( donorId );
                // If a test already exists, inform the donor and redirect them to an Edit page.
                TempData [ "Error" ] = "You already have a blood test. Please update your test if needed.";
                return RedirectToAction( "Edit", new { id = existingTest.Id } );
            }
            catch ( NotFoundException )
            {
                // No blood test exists, so allow the donor to create one.
            }

            var model = new CreateBloodTestDto { DonorId = donorId };

            // Retrieve the list of hospitals for the dropdown.
            var hospitals = await _userManager.GetUsersInRoleAsync( "Hospital" );
            ViewBag.Hospitals = hospitals
                .Select( h => new { h.Id, FullName = $"{h.FirstName} {h.LastName}" } )
                .ToList();

            return View( model );
        }

        // POST: BloodTest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create ( CreateBloodTestDto model )
        {
            if ( !ModelState.IsValid )
            {
                var hospitals = await _userManager.GetUsersInRoleAsync( "Hospital" );
                ViewBag.Hospitals = hospitals
                    .Select( h => new { h.Id, FullName = $"{h.FirstName} {h.LastName}" } )
                    .ToList();
                return View( model );
            }

            try
            {
                await _bloodTestService.CreateBloodTestAsync( model );
                TempData [ "Success" ] = "Blood test submitted successfully.";
                return RedirectToAction( "Index", "Donation" );
            }
            catch ( System.Exception ex )
            {
                ModelState.AddModelError( "", ex.Message );
                var hospitals = await _userManager.GetUsersInRoleAsync( "Hospital" );
                ViewBag.Hospitals = hospitals
                    .Select( h => new { h.Id, FullName = $"{h.FirstName} {h.LastName}" } )
                    .ToList();
                return View( model );
            }
        }

        // GET: BloodTest/Edit/5 (for updating an existing blood test)
        public async Task<IActionResult> Edit ( int id )
        {
            var bloodTestDto = await _bloodTestService.GetTestByIdAsync( id );
            if ( bloodTestDto == null )
            {
                TempData [ "Error" ] = "Blood test not found.";
                return RedirectToAction( "Create" );
            }

            // Retrieve hospitals for the dropdown.
            var hospitals = await _userManager.GetUsersInRoleAsync( "Hospital" );
            ViewBag.Hospitals = hospitals
                .Select( h => new { h.Id, FullName = $"{h.FirstName} {h.LastName}" } )
                .ToList();

            return View( bloodTestDto );
        }

        // POST: BloodTest/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit ( int id, UpdateBloodTestDto model )
        {
            if ( !ModelState.IsValid )
            {
                var hospitals = await _userManager.GetUsersInRoleAsync( "Hospital" );
                ViewBag.Hospitals = hospitals
                    .Select( h => new { h.Id, FullName = $"{h.FirstName} {h.LastName}" } )
                    .ToList();
                return View( model );
            }

            try
            {
                await _bloodTestService.UpdateTestResultsAsync( id, model );
                TempData [ "Success" ] = "Blood test updated successfully.";
                return RedirectToAction( "Index", "Donation" );
            }
            catch ( System.Exception ex )
            {
                ModelState.AddModelError( "", ex.Message );
                var hospitals = await _userManager.GetUsersInRoleAsync( "Hospital" );
                ViewBag.Hospitals = hospitals
                    .Select( h => new { h.Id, FullName = $"{h.FirstName} {h.LastName}" } )
                    .ToList();
                return View( model );
            }
        }
    }
}
