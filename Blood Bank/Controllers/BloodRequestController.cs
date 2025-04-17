using BloodBank.Core.Constants;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodBank.Web.Controllers
{
    [Authorize]
    public class BloodRequestController : Controller
    {
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IBloodUnitRepository _bloodUnitRepository;

        public BloodRequestController (
            IBloodRequestRepository bloodRequestRepository,
            IBloodUnitRepository bloodUnitRepository )
        {
            _bloodRequestRepository = bloodRequestRepository;
            _bloodUnitRepository = bloodUnitRepository;
        }

        [Authorize( Roles = Roles.Hospital )]
        public IActionResult Create ()
        {
            return View( new BloodRequest() );
        }

        [HttpPost]
        [Authorize( Roles = Roles.Hospital )]
        public async Task<IActionResult> Create ( BloodRequest model )
        {
            // Remove validation for properties not in the form
            ModelState.Remove( "Hospital" );
            ModelState.Remove( "AssignedUnits" );
            ModelState.Remove( "HospitalId" ); // Not needed in form; set programmatically

            if ( !ModelState.IsValid )
            {
                // Log invalid fields for debugging
                foreach ( var error in ModelState )
                {
                    if ( error.Value.Errors.Any() )
                    {
                        Console.WriteLine( $"{error.Key}: {string.Join( ", ", error.Value.Errors.Select( e => e.ErrorMessage ) )}" );
                    }
                }
                return View( model );
            }

            model.HospitalId = User.Identity.Name;
            model.RequestDate = DateTime.UtcNow;
            model.CreatedAt = DateTime.UtcNow;
            model.Status = RequestStatus.Pending;
            model.IsDeleted = false;
            model.AssignedUnits = new List<BloodUnit>();

            await _bloodRequestRepository.AddAsync( model );
            TempData [ "Success" ] = "Blood request created successfully.";
            return RedirectToAction( "Index" );
        }

        [Authorize( Roles = Roles.Hospital )]
        public async Task<IActionResult> Index ()
        {
            var requests = await _bloodRequestRepository.GetAllByHospitalIdAsync( User.Identity.Name );
            return View( requests );
        }

        [Authorize( Roles = Roles.Staff + "," + Roles.Admin )]
        public async Task<IActionResult> PendingRequests ()
        {
            var requests = await _bloodRequestRepository.GetPendingRequestsAsync();
            return View( requests );
        }

        [HttpPost]
        [Authorize( Roles = Roles.Staff + "," + Roles.Admin )]
        public async Task<IActionResult> Approve ( int id )
        {
            var request = await _bloodRequestRepository.GetByIdAsync( id );
            if ( request == null || request.Status != RequestStatus.Pending )
            {
                TempData [ "Error" ] = "Request not found or not in Pending status.";
                return RedirectToAction( "PendingRequests" );
            }

            request.Status = RequestStatus.Approved;
            request.UpdatedAt = DateTime.UtcNow;
            await _bloodRequestRepository.UpdateAsync( request );
            TempData [ "Success" ] = "Request approved successfully.";
            return RedirectToAction( "PendingRequests" );
        }

        [HttpPost]
        [Authorize( Roles = Roles.Staff + "," + Roles.Admin )]
        public async Task<IActionResult> Process ( int id )
        {
            var request = await _bloodRequestRepository.GetByIdAsync( id );
            if ( request == null || request.Status != RequestStatus.Approved )
            {
                TempData [ "Error" ] = "Request not found or not approved for processing.";
                return RedirectToAction( "PendingRequests" );
            }

            request.Status = RequestStatus.InProcess;
            var availableUnits = await _bloodUnitRepository.GetAvailableUnitsByBloodTypeAsync( request.BloodType );
            var requiredQuantity = request.QuantityRequired;
            var reservedUnits = new List<BloodUnit>();

            foreach ( var unit in availableUnits )
            {
                if ( requiredQuantity <= 0 ) break;

                reservedUnits.Add( unit );
                unit.Status = BloodUnitStatus.Reserved;
                requiredQuantity -= unit.Quantity;
                await _bloodUnitRepository.UpdateAsync( unit );
            }

            request.AssignedUnits = reservedUnits;

            if ( requiredQuantity > 0 )
            {
                request.Status = RequestStatus.InProcess;
                TempData [ "Warning" ] = "Request partially fulfilled; awaiting more units.";
            }
            else
            {
                request.Status = RequestStatus.Completed;
                foreach ( var unit in reservedUnits )
                {
                    unit.Status = BloodUnitStatus.Dispatched;
                    await _bloodUnitRepository.UpdateAsync( unit );
                }
                TempData [ "Success" ] = "Request fully processed and dispatched.";
            }

            request.UpdatedAt = DateTime.UtcNow;
            await _bloodRequestRepository.UpdateAsync( request );
            return RedirectToAction( "PendingRequests" );
        }

        [HttpPost]
        [Authorize( Roles = Roles.Staff + "," + Roles.Admin )]
        public async Task<IActionResult> Reject ( int id, string rejectionReason )
        {
            var request = await _bloodRequestRepository.GetByIdAsync( id );
            if ( request == null || ( request.Status != RequestStatus.Pending && request.Status != RequestStatus.Approved ) )
            {
                TempData [ "Error" ] = "Request not found or cannot be rejected.";
                return RedirectToAction( "PendingRequests" );
            }

            request.Status = RequestStatus.Rejected;
            request.Notes = $"{request.Notes}\nRejection Reason: {rejectionReason ?? "No reason provided."}".Trim();
            request.UpdatedAt = DateTime.UtcNow;
            await _bloodRequestRepository.UpdateAsync( request );
            TempData [ "Success" ] = "Request rejected successfully.";
            return RedirectToAction( "PendingRequests" );
        }
    }
}