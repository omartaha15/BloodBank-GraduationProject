using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blood_Bank.Controllers
{
    // BloodBank.Web/Controllers/AccountController.cs
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly UserManager<User> _userManager;

        public AccountController (
            IAuthService authService,
            UserManager<User> userManager )
        {
            _authService = authService;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Register ()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register ( RegisterDto model )
        {
            if ( !ModelState.IsValid )
            {
                return View( model );
            }

            try
            {
                await _authService.RegisterAsync( model );
                return RedirectToAction( nameof( Login ) );
            }
            catch ( Exception ex )
            {
                ModelState.AddModelError( "", ex.Message );
                return View( model );
            }
        }

        [HttpGet]
        public IActionResult Login ()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login ( LoginDto model )
        {
            if ( !ModelState.IsValid )
            {
                return View( model );
            }

            try
            {
                await _authService.LoginAsync( model );
                return RedirectToAction( "Index", "Home" );
            }
            catch ( Exception ex )
            {
                ModelState.AddModelError( "", ex.Message );
                return View( model );
            }
        }

        [HttpPost]
        public async Task<IActionResult> Logout ()
        {
            await _authService.LogoutAsync();
            return RedirectToAction( "Index", "Home" );
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword ()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword ( ChangePasswordDto model )
        {
            if ( !ModelState.IsValid )
            {
                return View( model );
            }

            try
            {
                var userId = _userManager.GetUserId( User );
                await _authService.ChangePasswordAsync( userId, model );

                // Add success message
                TempData [ "SuccessMessage" ] = "Password changed successfully.";
                return RedirectToAction( "Index", "Home" );
            }
            catch ( Exception ex )
            {
                ModelState.AddModelError( "", ex.Message );
                return View( model );
            }
        }
    }
}
