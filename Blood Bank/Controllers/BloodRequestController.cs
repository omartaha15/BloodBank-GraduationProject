using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Web.Controllers
{
    public class BloodRequestController : Controller
    {
        private readonly BloodBankDbContext _context;
        private readonly UserManager<User> _userManager;

        public BloodRequestController ( BloodBankDbContext context, UserManager<User> userManager )
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BloodRequests/Create
        [Authorize( Roles = "Hospital" )]
        public IActionResult Create ()
        {
            return View();
        }

        // POST: BloodRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Hospital" )]
        public async Task<IActionResult> Create ( BloodRequest model )
        {
            ModelState.Remove( nameof( model.Hospital ) );
            if ( ModelState.IsValid )
            {
                var user = await _userManager.GetUserAsync( User );
                model.HospitalId = user.Id;
                model.Status = RequestStatus.Pending;
                model.RequestDate = DateTime.Now;
                _context.Add( model );
                await _context.SaveChangesAsync();
                return RedirectToAction( nameof( MyRequests ) );
            }
            return View( model );
        }

        // GET: BloodRequests/MyRequests
        [Authorize( Roles = "Hospital" )]
        public async Task<IActionResult> MyRequests ()
        {
            var user = await _userManager.GetUserAsync( User );
            var requests = await _context.BloodRequests
                .Where( r => r.HospitalId == user.Id )
                .ToListAsync();
            return View( requests );
        }

        // GET: BloodRequests/Manage
        [Authorize( Roles = "Admin" )]
        public async Task<IActionResult> Manage ()
        {
            var requests = await _context.BloodRequests
                .Where( r => r.Status == RequestStatus.Pending )
                .Include( r => r.Hospital )
                .ToListAsync();
            return View( requests );
        }

        // GET: BloodRequests/Details/5
        [Authorize( Roles = "Admin,Hospital" )]
        public async Task<IActionResult> Details ( int id )
        {
            var request = await _context.BloodRequests
                .Include( r => r.Hospital )
                .Include( r => r.AssignedUnits )
                .FirstOrDefaultAsync( r => r.Id == id );
            if ( request == null )
            {
                return NotFound();
            }

            if ( User.IsInRole( "Hospital" ) )
            {
                var user = await _userManager.GetUserAsync( User );
                if ( request.HospitalId != user.Id )
                {
                    return Forbid();
                }
            }

            var availableUnits = await _context.BloodUnits
                .Where( u => u.BloodType == request.BloodType && u.Status == BloodUnitStatus.Available )
                .ToListAsync();
            var availableQuantity = availableUnits.Sum( u => u.Quantity );
            ViewBag.AvailableQuantity = availableQuantity;

            return View( request );
        }

        // POST: BloodRequests/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin" )]
        public async Task<IActionResult> Approve ( int id )
        {
            var request = await _context.BloodRequests
                .Include( r => r.AssignedUnits )
                .FirstOrDefaultAsync( r => r.Id == id );
            if ( request == null )
            {
                return NotFound();
            }

            var availableUnits = await _context.BloodUnits
                .Where( u => u.BloodType == request.BloodType && u.Status == BloodUnitStatus.Available )
                .OrderBy( u => u.ExpiryDate )
                .ToListAsync();
            var availableQuantity = availableUnits.Sum( u => u.Quantity );

            if ( availableQuantity < request.QuantityRequired )
            {
                TempData [ "Error" ] = "Not enough blood units available to fulfill the request.";
                return RedirectToAction( nameof( Details ), new { id = id } );
            }

            double required = request.QuantityRequired;
            double assigned = 0;
            var toAssign = new List<BloodUnit>();
            foreach ( var unit in availableUnits )
            {
                if ( assigned >= required ) break;
                toAssign.Add( unit );
                assigned += unit.Quantity;
            }

            foreach ( var unit in toAssign )
            {
                unit.Status = BloodUnitStatus.Reserved;
                request.AssignedUnits.Add( unit );
            }
            request.Status = RequestStatus.Approved;
            await _context.SaveChangesAsync();
            return RedirectToAction( nameof( Manage ) );
        }

        // POST: BloodRequests/Reject/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin" )]
        public async Task<IActionResult> Reject ( int id )
        {
            var request = await _context.BloodRequests.FindAsync( id );
            if ( request == null )
            {
                return NotFound();
            }
            request.Status = RequestStatus.Rejected;
            await _context.SaveChangesAsync();
            return RedirectToAction( nameof( Manage ) );
        }
    }
}