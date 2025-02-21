using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using BloodBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Repositories
{
    public class DonationRepository : GenericRepository<Donation>, IDonationRepository
    {
        public DonationRepository ( BloodBankDbContext context ) : base( context )
        {
        }

        public async Task<IEnumerable<Donation>> GetDonationsByDonorIdAsync ( string donorId )
        {
            return await _dbSet
                .Where( d => d.DonorId == donorId && !d.IsDeleted )
                .Include( d => d.BloodTest )
                .Include( d => d.BloodUnit )
                .OrderByDescending( d => d.DonationDate )
                .ToListAsync();
        }

        public async Task<IEnumerable<Donation>> GetDonationsByBloodTypeAsync ( BloodType bloodType )
        {
            return await _dbSet
                .Where( d => d.BloodType == bloodType && !d.IsDeleted )
                .Include( d => d.BloodTest )
                .Include( d => d.BloodUnit )
                .OrderByDescending( d => d.DonationDate )
                .ToListAsync();
        }

        public async Task<IEnumerable<Donation>> GetRecentDonationsAsync ( int count )
        {
            return await _dbSet
                .Where( d => !d.IsDeleted )
                .Include( d => d.Donor )
                .Include( d => d.BloodTest )
                .OrderByDescending( d => d.DonationDate )
                .Take( count )
                .ToListAsync();
        }
    }
}
