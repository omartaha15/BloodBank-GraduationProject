using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blood_Bank.Controllers
{
    [Authorize]
    public class BloodRequestController : Controller
    {
        private readonly IBloodRequestService _bloodRequestService;
        private readonly IBloodUnitService _bloodUnitService;
        private readonly UserManager<User> _userManager;

        public BloodRequestController (
            IBloodRequestService bloodRequestService,
            IBloodUnitService bloodUnitService,
            UserManager<User> userManager )
        {
            _bloodRequestService = bloodRequestService;
            _bloodUnitService = bloodUnitService;
            _userManager = userManager;
        }

        // GET: BloodRequest
        public async Task<IActionResult> Index ()
        {
            var requests = await _bloodRequestService.GetAllRequestsAsync();
            return View( requests );
        }

        // GET: BloodRequest/Create
        [Authorize( Roles = "Hospital" )]
        public async Task<IActionResult> Create ()
        {
            // Get available blood types and their quantities for the dropdown
            var bloodTypes = Enum.GetValues( typeof( BloodType ) )
                                .Cast<BloodType>()
                                .Select( async bt => new
                                {
                                    BloodType = bt,
                                    AvailableUnits = await _bloodUnitService.GetAvailableUnitsCountByBloodTypeAsync( bt )
                                } )
                                .Select( t => t.Result )
                                .ToList();

            ViewBag.BloodTypes = bloodTypes;
            return View( new CreateBloodRequestDto() );
        }

        // POST: BloodRequest/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Hospital" )]
        public async Task<IActionResult> Create ( CreateBloodRequestDto requestDto )
        {
            if ( !ModelState.IsValid )
                return View( requestDto );

            try
            {
                // Set the hospital ID from the current user
                requestDto.HospitalId = int.Parse( _userManager.GetUserId( User ) );

                await _bloodRequestService.CreateRequestAsync( requestDto );
                TempData [ "Success" ] = "Blood request created successfully.";
                return RedirectToAction( nameof( Index ) );
            }
            catch ( Exception ex )
            {
                ModelState.AddModelError( "", ex.Message );
                return View( requestDto );
            }
        }

        // GET: BloodRequest/Details/5
        public async Task<IActionResult> Details ( int id )
        {
            var request = await _bloodRequestService.GetRequestByIdAsync( id );
            if ( request == null )
                return NotFound();

            return View( request );
        }

        // GET: BloodRequest/MyRequests
        [Authorize( Roles = "Hospital" )]
        public async Task<IActionResult> MyRequests ()
        {
            var hospitalId = int.Parse( _userManager.GetUserId( User ) );
            var requests = await _bloodRequestService.GetHospitalRequestsAsync( hospitalId );
            return View( requests );
        }

        // POST: BloodRequest/UpdateStatus/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin,Staff" )]
        public async Task<IActionResult> UpdateStatus ( int id, RequestStatus status )
        {
            try
            {
                await _bloodRequestService.UpdateRequestStatusAsync( id, status );
                TempData [ "Success" ] = "Request status updated successfully.";
            }
            catch ( Exception ex )
            {
                TempData [ "Error" ] = ex.Message;
            }
            return RedirectToAction( nameof( Details ), new { id } );
        }

        // GET: BloodRequest/Urgent
        [Authorize( Roles = "Admin,Staff" )]
        public async Task<IActionResult> Urgent ()
        {
            var urgentRequests = await _bloodRequestService.GetUrgentRequestsAsync();
            return View( urgentRequests );
        }
    }
}
