using AutoMapper;
using BloodBank.Business.DTOs;
using BloodBank.Core.Constants;
using BloodBank.Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blood_Bank.Controllers
{
    //[Area( "Admin" )]
    [Authorize( Roles = Roles.Admin )]
    public class UserManagementController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UserManagementController (
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IMapper mapper )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index ()
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

        public async Task<IActionResult> ChangeRole ( string userId )
        {
            var user = await _userManager.FindByIdAsync( userId );
            if ( user == null )
                return NotFound();

            var model = new ChangeRoleDto
            {
                UserId = userId,
                UserEmail = user.Email,
                CurrentRoles = await _userManager.GetRolesAsync( user ),
                AvailableRoles = Roles.All.ToList()
            };

            return View( model );
        }

        [HttpPost]
        public async Task<IActionResult> ChangeRole ( ChangeRoleDto model )
        {
            if ( !ModelState.IsValid )
                return View( model );

            var user = await _userManager.FindByIdAsync( model.UserId );
            if ( user == null )
                return NotFound();

            var currentRoles = await _userManager.GetRolesAsync( user );
            await _userManager.RemoveFromRolesAsync( user, currentRoles );
            await _userManager.AddToRoleAsync( user, model.NewRole );

            return RedirectToAction( nameof( Index ) );
        }
    }
}
