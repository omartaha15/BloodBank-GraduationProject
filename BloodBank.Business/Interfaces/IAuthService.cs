using BloodBank.Business.DTOs;

namespace BloodBank.Business.Interfaces
{
    
    public interface IAuthService
    {
        Task<bool> RegisterAsync ( RegisterDto model );
        Task<bool> LoginAsync ( LoginDto model );
        Task LogoutAsync ();
        Task<bool> ChangePasswordAsync ( string userId, ChangePasswordDto model );
    }
}
