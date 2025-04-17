using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BloodBank.Web.Controllers
{
    [Authorize( Roles = "Hospital,Admin,Staff" )]
    public class InventoryController : Controller
    {
        private readonly BloodBankDbContext _context;

        public InventoryController ( BloodBankDbContext context )
        {
            _context = context;
        }

        public async Task<IActionResult> Index ()
        {
            var units = await _context.BloodUnits
                .Where( u => !u.IsDeleted )
                .Include( u => u.Donation )
                .ToListAsync();
            return View( units );
        }

        public IActionResult Create ()
        {
            ViewBag.BloodTypes = Enum.GetValues( typeof( BloodType ) ).Cast<BloodType>();
            ViewBag.Statuses = Enum.GetValues( typeof( BloodUnitStatus ) ).Cast<BloodUnitStatus>();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create ( BloodUnit model )
        {
            if ( !ModelState.IsValid )
            {
                ViewBag.BloodTypes = Enum.GetValues( typeof( BloodType ) ).Cast<BloodType>();
                ViewBag.Statuses = Enum.GetValues( typeof( BloodUnitStatus ) ).Cast<BloodUnitStatus>();
                return View( model );
            }

            model.UnitNumber = $"BU{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString().Substring( 0, 8 )}";
            model.CreatedAt = DateTime.UtcNow;
            model.IsDeleted = false;

            _context.BloodUnits.Add( model );
            await _context.SaveChangesAsync();
            TempData [ "Success" ] = "Blood unit added successfully.";
            return RedirectToAction( nameof( Index ) );
        }

        public async Task<IActionResult> Edit ( int id )
        {
            var unit = await _context.BloodUnits.FindAsync( id );
            if ( unit == null || unit.IsDeleted )
            {
                TempData [ "Error" ] = "Blood unit not found.";
                return RedirectToAction( nameof( Index ) );
            }

            ViewBag.BloodTypes = Enum.GetValues( typeof( BloodType ) ).Cast<BloodType>();
            ViewBag.Statuses = Enum.GetValues( typeof( BloodUnitStatus ) ).Cast<BloodUnitStatus>();
            return View( unit );
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit ( int id, BloodUnit model )
        {
            if ( id != model.Id )
            {
                TempData [ "Error" ] = "Invalid blood unit ID.";
                return RedirectToAction( nameof( Index ) );
            }

            if ( !ModelState.IsValid )
            {
                ViewBag.BloodTypes = Enum.GetValues( typeof( BloodType ) ).Cast<BloodType>();
                ViewBag.Statuses = Enum.GetValues( typeof( BloodUnitStatus ) ).Cast<BloodUnitStatus>();
                return View( model );
            }

            var unit = await _context.BloodUnits.FindAsync( id );
            if ( unit == null || unit.IsDeleted )
            {
                TempData [ "Error" ] = "Blood unit not found.";
                return RedirectToAction( nameof( Index ) );
            }

            unit.BloodType = model.BloodType;
            unit.Quantity = model.Quantity;
            unit.Status = model.Status;
            unit.StorageLocation = model.StorageLocation;
            unit.ExpiryDate = model.ExpiryDate;
            unit.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            TempData [ "Success" ] = "Blood unit updated successfully.";
            return RedirectToAction( nameof( Index ) );
        }

        public async Task<IActionResult> Delete ( int id )
        {
            var unit = await _context.BloodUnits.FindAsync( id );
            if ( unit == null || unit.IsDeleted )
            {
                TempData [ "Error" ] = "Blood unit not found.";
                return RedirectToAction( nameof( Index ) );
            }

            return View( unit );
        }

        [HttpPost, ActionName( "Delete" )]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed ( int id )
        {
            var unit = await _context.BloodUnits.FindAsync( id );
            if ( unit == null || unit.IsDeleted )
            {
                TempData [ "Error" ] = "Blood unit not found.";
                return RedirectToAction( nameof( Index ) );
            }

            unit.IsDeleted = true;
            unit.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            TempData [ "Success" ] = "Blood unit deleted successfully.";
            return RedirectToAction( nameof( Index ) );
        }
    }
}
