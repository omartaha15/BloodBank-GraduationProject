using BloodBank.Core.Entities;
using BloodBank.Core.Enums;

namespace BloodBank.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync ( string id );
        Task<IEnumerable<User>> GetAllAsync ();
        Task<User> GetByEmailAsync ( string email );
        Task<IEnumerable<User>> GetDonorsAsync ();
        Task<User> GetUserWithDonationsAsync ( string id );
        Task<bool> HasRecentDonationAsync ( string userId, int monthsThreshold );
        Task<IEnumerable<User>> GetUsersByBloodTypeAsync ( BloodType bloodType );
        Task<bool> IsEmailUniqueAsync ( string email, string excludeUserId = null );
        Task<bool> IsPhoneUniqueAsync ( string phone, string excludeUserId = null );
    }
}
