using BloodBank.Core.Entities;
using BloodBank.Core.Interfaces;
using BloodBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Infrastructure.Repositories
{
    public class BloodTestRepository : GenericRepository<BloodTest>, IBloodTestRepository
    {
        public BloodTestRepository ( BloodBankDbContext context ) : base( context )
        {
        }

        public async Task<BloodTest> GetTestByDonationIdAsync ( int donationId )
        {
            return await _dbSet
                .FirstOrDefaultAsync( bt => bt.DonationId == donationId && !bt.IsDeleted );
        }

        public async Task<IEnumerable<BloodTest>> GetRecentTestsAsync ( int count )
        {
            return await _dbSet
                .Where( bt => !bt.IsDeleted )
                .Include( bt => bt.Donation )
                .OrderByDescending( bt => bt.TestDate )
                .Take( count )
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodTest>> GetPendingTestsAsync ()
        {
            return await _dbSet
                .Where( bt => !bt.IsDeleted && !bt.IsTestPassed )
                .Include( bt => bt.Donation )
                .OrderBy( bt => bt.TestDate )
                .ToListAsync();
        }
    }
}
