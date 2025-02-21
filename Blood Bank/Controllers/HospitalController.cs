using AutoMapper;
using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blood_Bank.Controllers
{
    // Controllers/HospitalController.cs
    [Authorize]
    public class HospitalController : Controller
    {
        private readonly IHospitalService _hospitalService;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public HospitalController (
            IHospitalService hospitalService,
            UserManager<User> userManager,
            IMapper mapper )
        {
            _hospitalService = hospitalService;
            _userManager = userManager;
            _mapper = mapper;
        }

        // GET: Hospital
        public async Task<IActionResult> Index ()
        {
            var hospitals = await _hospitalService.GetAllHospitalsAsync();
            return View( hospitals );
        }

        // GET: Hospital/Create
        [Authorize( Roles = "Admin" )]
        public IActionResult Create ()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin" )]
        public async Task<IActionResult> Create ( CreateHospitalDto hospitalDto )
        {
            if ( !ModelState.IsValid )
                return View( hospitalDto );

            try
            {
                // Create hospital
                var hospital = await _hospitalService.CreateHospitalAsync( hospitalDto );

                // Create user account for hospital
                var user = new User
                {
                    UserName = hospitalDto.Email,
                    Email = hospitalDto.Email,
                    FirstName = hospitalDto.Name,
                    LastName = "Hospital",
                    PhoneNumber = hospitalDto.ContactNumber,
                    EmailConfirmed = true // Since admin is creating it
                };

                // Generate a random password (you should send this to the hospital's email)
                var password = GenerateRandomPassword();
                var result = await _userManager.CreateAsync( user, password );

                if ( result.Succeeded )
                {
                    // Assign Hospital role
                    await _userManager.AddToRoleAsync( user, "Hospital" );

                    // TODO: Send email to hospital with their credentials

                    TempData [ "Success" ] = "Hospital created successfully. Login credentials have been sent to the provided email.";
                }
                else
                {
                    // If user creation fails, you might want to delete the hospital record
                    ModelState.AddModelError( "", "Failed to create hospital account." );
                    return View( hospitalDto );
                }

                return RedirectToAction( nameof( Index ) );
            }
            catch ( Exception ex )
            {
                ModelState.AddModelError( "", ex.Message );
                return View( hospitalDto );
            }
        }

        private string GenerateRandomPassword ()
        {
            // Generate a secure random password
            return Guid.NewGuid().ToString( "N" ).Substring( 0, 8 ) + "X@1a";
        }

        // GET: Hospital/Details/5
        public async Task<IActionResult> Details ( int id )
        {
            var hospital = await _hospitalService.GetHospitalWithRequestsAsync( id );
            if ( hospital == null )
                return NotFound();

            // Get hospital's blood requests
            var requests = await _hospitalService.GetHospitalRequestsAsync( id );
            ViewBag.BloodRequests = requests;

            return View( hospital );
        }

        // GET: Hospital/Edit/5
        [Authorize( Roles = "Admin" )]
        public async Task<IActionResult> Edit ( int id )
        {
            var hospital = await _hospitalService.GetHospitalByIdAsync( id );
            if ( hospital == null )
                return NotFound();

            var updateDto = _mapper.Map<UpdateHospitalDto>( hospital );
            return View( updateDto );
        }

        // POST: Hospital/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin" )]
        public async Task<IActionResult> Edit ( int id, UpdateHospitalDto hospitalDto )
        {
            if ( !ModelState.IsValid )
                return View( hospitalDto );

            try
            {
                await _hospitalService.UpdateHospitalAsync( id, hospitalDto );
                TempData [ "Success" ] = "Hospital updated successfully.";
                return RedirectToAction( nameof( Index ) );
            }
            catch ( Exception ex )
            {
                ModelState.AddModelError( "", ex.Message );
                return View( hospitalDto );
            }
        }

        // POST: Hospital/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize( Roles = "Admin" )]
        public async Task<IActionResult> Delete ( int id )
        {
            try
            {
                await _hospitalService.DeleteHospitalAsync( id );
                TempData [ "Success" ] = "Hospital deleted successfully.";
            }
            catch ( Exception ex )
            {
                TempData [ "Error" ] = ex.Message;
            }
            return RedirectToAction( nameof( Index ) );
        }

        // GET: Hospital/Requests/5
        [Authorize( Roles = "Admin,Hospital" )]
        public async Task<IActionResult> Requests ( int id )
        {
            var requests = await _hospitalService.GetHospitalRequestsAsync( id );
            ViewBag.HospitalId = id;
            var hospital = await _hospitalService.GetHospitalByIdAsync( id );
            ViewBag.HospitalName = hospital?.Name;
            return View( requests );
        }

        // POST: Hospital/ValidateCredentials
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateCredentials ( string email, string contactNumber )
        {
            var isValid = await _hospitalService.ValidateHospitalCredentialsAsync( email, contactNumber );
            return Json( new { isValid } );
        }
    }
}
