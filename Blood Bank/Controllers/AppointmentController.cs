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
    public class AppointmentController : Controller
    {
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<User> _userManager;

        public AppointmentController (
            IAppointmentService appointmentService,
            UserManager<User> userManager )
        {
            _appointmentService = appointmentService;
            _userManager = userManager;
        }

        // GET: Appointment
        public async Task<IActionResult> Index ()
        {
            var appointments = await _appointmentService.GetAllAppointmentsAsync();
            return View( appointments );
        }

        // GET: Appointment/Create
        public IActionResult Create ()
        {
            var createDto = new CreateAppointmentDto
            {
                DonorId = _userManager.GetUserId( User )
            };
            return View( createDto );
        }

        // POST: Appointment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create ( CreateAppointmentDto appointmentDto )
        {
            if ( !ModelState.IsValid )
                return View( appointmentDto );

            try
            {
                // Check if time slot is available
                if ( !await _appointmentService.IsTimeSlotAvailableAsync( appointmentDto.AppointmentDate, appointmentDto.AppointmentTime ) )
                {
                    ModelState.AddModelError( "", "This time slot is not available. Please choose another time." );
                    return View( appointmentDto );
                }

                await _appointmentService.CreateAppointmentAsync( appointmentDto );
                TempData [ "Success" ] = "Appointment scheduled successfully.";
                return RedirectToAction( nameof( Index ) );
            }
            catch ( Exception ex )
            {
                ModelState.AddModelError( "", ex.Message );
                return View( appointmentDto );
            }
        }

        // GET: Appointment/Details/5
        public async Task<IActionResult> Details ( int id )
        {
            var appointment = await _appointmentService.GetAppointmentByIdAsync( id );
            if ( appointment == null )
                return NotFound();

            return View( appointment );
        }

        // GET: Appointment/MyAppointments
        [Authorize( Roles = "Donor" )]
        public async Task<IActionResult> MyAppointments ()
        {
            var userId = _userManager.GetUserId( User );
            var appointments = await _appointmentService.GetDonorAppointmentsAsync( userId );
            return View( appointments );
        }

        // POST: Appointment/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel ( int id )
        {
            try
            {
                await _appointmentService.UpdateAppointmentStatusAsync( id, AppointmentStatus.Cancelled );
                TempData [ "Success" ] = "Appointment cancelled successfully.";
                return RedirectToAction( nameof( Index ) );
            }
            catch ( Exception ex )
            {
                TempData [ "Error" ] = ex.Message;
                return RedirectToAction( nameof( Index ) );
            }
        }
    }
}
