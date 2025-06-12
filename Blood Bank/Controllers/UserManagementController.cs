using AutoMapper;
using BloodBank.Business.DTOs;
using BloodBank.Core.Constants;
using BloodBank.Core.Entities;
using BloodBank.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blood_Bank.Controllers
{
    [Authorize( Roles = Roles.Admin )]
    public class UserManagementController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;
        private readonly ILogger<UserManagementController> _logger;
        private readonly BloodBankDbContext _context;

        public UserManagementController (
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            SignInManager<User> signInManager,
            IMapper mapper,
            ILogger<UserManagementController> logger,
            BloodBankDbContext bloodBankDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _logger = logger;
            _context = bloodBankDbContext;
        }

        public async Task<IActionResult> Index ()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                var userDtos = new List<UserManagementDto>();

                foreach ( var user in users )
                {
                    var roles = await _userManager.GetRolesAsync( user );
                    var userDto = _mapper.Map<UserManagementDto>( user );
                    userDto.Roles = roles;
                    userDtos.Add( userDto );
                }

                return View( userDtos );
            }
            catch ( Exception ex )
            {
                _logger.LogError( ex, "Error fetching users." );
                TempData [ "ErrorMessage" ] = "Error loading users.";
                return View( new List<UserManagementDto>() );
            }
        }

        public async Task<IActionResult> GetAllUsers ()
        {
            var users = _userManager.Users.ToList();
            var userWithRoles = new List<UserWithRoleViewModel>();

            foreach ( var user in users )
            {
                var roles = await _userManager.GetRolesAsync( user );
                userWithRoles.Add( new UserWithRoleViewModel
                {
                    User = user,
                    Roles = roles.ToList()
                } );
            }

            return View( userWithRoles );
        }

        public async Task<IActionResult> ChangeRole ( string userId )
        {
            if ( string.IsNullOrWhiteSpace( userId ) )
            {
                TempData [ "ErrorMessage" ] = "User ID not provided.";
                return RedirectToAction( nameof( Index ) );
            }

            try
            {
                var user = await _userManager.FindByIdAsync( userId );
                if ( user == null )
                {
                    TempData [ "ErrorMessage" ] = "User not found.";
                    return RedirectToAction( nameof( Index ) );
                }

                var model = new ChangeRoleDto
                {
                    UserId = userId,
                    UserEmail = user.Email,
                    CurrentRoles = await _userManager.GetRolesAsync( user ),
                    AvailableRoles = Roles.All.ToList()
                };

                return View( model );
            }
            catch ( Exception ex )
            {
                _logger.LogError( ex, "Error loading change role form." );
                TempData [ "ErrorMessage" ] = "Error loading form.";
                return RedirectToAction( nameof( Index ) );
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole ( ChangeRoleDto model )
        {
            // Remove properties that are not posted from ModelState.
            ModelState.Remove( nameof( model.UserEmail ) );
            ModelState.Remove( nameof( model.CurrentRoles ) );
            ModelState.Remove( nameof( model.AvailableRoles ) );

            if ( !ModelState.IsValid )
            {
                // Re-populate display-only properties from the database.
                var userForValidation = await _userManager.FindByIdAsync( model.UserId );
                if ( userForValidation != null )
                {
                    model.UserEmail = userForValidation.Email;
                    model.CurrentRoles = await _userManager.GetRolesAsync( userForValidation );
                }
                model.AvailableRoles = Roles.All.ToList();
                return View( model );
            }

            var user = await _userManager.FindByIdAsync( model.UserId );
            if ( user == null )
            {
                TempData [ "ErrorMessage" ] = "User not found.";
                return RedirectToAction( nameof( Index ) );
            }

            if ( string.IsNullOrWhiteSpace( model.NewRole ) )
            {
                ModelState.AddModelError( "NewRole", "Please select a new role." );
                model.UserEmail = user.Email;
                model.CurrentRoles = await _userManager.GetRolesAsync( user );
                model.AvailableRoles = Roles.All.ToList();
                return View( model );
            }

            if ( !await _roleManager.RoleExistsAsync( model.NewRole ) )
            {
                ModelState.AddModelError( "NewRole", "The selected role does not exist." );
                model.UserEmail = user.Email;
                model.CurrentRoles = await _userManager.GetRolesAsync( user );
                model.AvailableRoles = Roles.All.ToList();
                return View( model );
            }

            var currentRoles = await _userManager.GetRolesAsync( user );
            if ( currentRoles.Contains( model.NewRole ) )
            {
                TempData [ "SuccessMessage" ] = "User already has the selected role.";
                return RedirectToAction( nameof( Index ) );
            }

            if ( currentRoles.Any() )
            {
                var removeResult = await _userManager.RemoveFromRolesAsync( user, currentRoles );
                if ( !removeResult.Succeeded )
                {
                    ModelState.AddModelError( "", "Error removing current roles." );
                    model.UserEmail = user.Email;
                    model.CurrentRoles = currentRoles;
                    model.AvailableRoles = Roles.All.ToList();
                    return View( model );
                }
            }

            var addResult = await _userManager.AddToRoleAsync( user, model.NewRole );
            if ( !addResult.Succeeded )
            {
                ModelState.AddModelError( "", "Error adding new role." );
                model.UserEmail = user.Email;
                model.CurrentRoles = await _userManager.GetRolesAsync( user );
                model.AvailableRoles = Roles.All.ToList();
                return View( model );
            }

            await _userManager.UpdateSecurityStampAsync( user );
            if ( user.Id == _userManager.GetUserId( User ) )
            {
                await _signInManager.RefreshSignInAsync( user );
            }

            TempData [ "SuccessMessage" ] = "Role changed successfully.";
            return RedirectToAction( nameof( Index ) );
        }


    }

    public class UserWithRoleViewModel
    {
        public User User { get; set; }
        public List<string> Roles { get; set; }
    }

}
