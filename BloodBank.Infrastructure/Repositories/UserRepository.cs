using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using BloodBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly BloodBankDbContext _context;

        public UserRepository ( BloodBankDbContext context )
        {
            _context = context;
        }

        public async Task<User> GetByIdAsync ( string id )
        {
            return await _context.Users
                .FirstOrDefaultAsync( u => u.Id == id );
        }

        public async Task<IEnumerable<User>> GetAllAsync ()
        {
            return await _context.Users
                .OrderBy( u => u.FirstName )
                .ThenBy( u => u.LastName )
                .ToListAsync();
        }

        public async Task<User> GetByEmailAsync ( string email )
        {
            return await _context.Users
                .FirstOrDefaultAsync( u => u.Email == email );
        }

        public async Task<IEnumerable<User>> GetDonorsAsync ()
        {
            var donorRoleId = await _context.Roles
                .Where( r => r.Name == "Donor" )
                .Select( r => r.Id )
                .FirstOrDefaultAsync();

            return await _context.Users
                .Where( u => _context.UserRoles.Any( ur => ur.UserId == u.Id &&
                                                        ur.RoleId == donorRoleId ) )
                .OrderBy( u => u.FirstName )
                .ThenBy( u => u.LastName )
                .ToListAsync();
        }

        public async Task<User> GetUserWithDonationsAsync ( string id )
        {
            return await _context.Users
                .Include( u => u.Donations )
                .Include( u => u.Donations )
                    .ThenInclude( d => d.BloodUnit )
                .FirstOrDefaultAsync( u => u.Id == id );
        }

        public async Task<bool> HasRecentDonationAsync ( string userId, int monthsThreshold )
        {
            var thresholdDate = DateTime.UtcNow.AddMonths( -monthsThreshold );

            return await _context.Donations
                .AnyAsync( d => d.DonorId == userId &&
                              !d.IsDeleted &&
                              d.DonationDate >= thresholdDate &&
                              d.Status == DonationStatus.Approved );
        }

        public async Task<IEnumerable<User>> GetUsersByBloodTypeAsync ( BloodType bloodType )
        {
            return await _context.Users
                .Where( u => u.BloodType == bloodType )
                .OrderBy( u => u.FirstName )
                .ThenBy( u => u.LastName )
                .ToListAsync();
        }

        public async Task<bool> IsEmailUniqueAsync ( string email, string excludeUserId = null )
        {
            var query = _context.Users.Where( u => u.Email == email );

            if ( !string.IsNullOrEmpty( excludeUserId ) )
            {
                query = query.Where( u => u.Id != excludeUserId );
            }

            return !await query.AnyAsync();
        }

        public async Task<bool> IsPhoneUniqueAsync ( string phone, string excludeUserId = null )
        {
            var query = _context.Users.Where( u => u.PhoneNumber == phone );

            if ( !string.IsNullOrEmpty( excludeUserId ) )
            {
                query = query.Where( u => u.Id != excludeUserId );
            }

            return !await query.AnyAsync();
        }
    }
}
