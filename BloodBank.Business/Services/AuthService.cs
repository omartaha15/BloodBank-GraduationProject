using AutoMapper;
using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using Microsoft.AspNetCore.Identity;
using OpenQA.Selenium;

namespace BloodBank.Business.Services
{
    // BloodBank.Business/Services/AuthService.cs
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IMapper _mapper;

        public AuthService (
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IMapper mapper )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
        }

        public async Task<bool> RegisterAsync ( RegisterDto model )
        {
            var user = _mapper.Map<User>( model );
            user.UserName = model.Email; // Set username to email

            var result = await _userManager.CreateAsync( user, model.Password );
            if ( !result.Succeeded )
            {
                throw new Exception( result.Errors.First().Description );
            }

            // Add default role
            await _userManager.AddToRoleAsync( user, "Donor" );

            return true;
        }

        public async Task<bool> LoginAsync ( LoginDto model )
        {
            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                isPersistent: false,
                lockoutOnFailure: false );

            if ( !result.Succeeded )
            {
                throw new Exception( "Invalid login attempt." );
            }

            return true;
        }

        public async Task LogoutAsync ()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<bool> ChangePasswordAsync ( string userId, ChangePasswordDto model )
        {
            var user = await _userManager.FindByIdAsync( userId );
            if ( user == null )
            {
                throw new Exception( "User not found." );
            }

            if ( model.NewPassword != model.ConfirmNewPassword )
            {
                throw new Exception( "New password and confirmation password do not match." );
            }

            var result = await _userManager.ChangePasswordAsync(
                user,
                model.CurrentPassword,
                model.NewPassword );

            if ( !result.Succeeded )
            {
                throw new Exception( result.Errors.First().Description );
            }

            // Optional: Sign in again after password change
            await _signInManager.SignInAsync( user, isPersistent: false );

            return true;
        }
    }

}
