using BloodBank.Core.Entities;
using BloodBank.Core.Entities.BloodBank.Core.Entities;
using BloodBank.Core.Interfaces;
using BloodBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodBank.Infrastructure.Repositories
{
    public class BloodTestRepository : GenericRepository<BloodTest>, IBloodTestRepository
    {
        public BloodTestRepository ( BloodBankDbContext context ) : base( context )
        {
        }

        public async Task<BloodTest> GetTestByDonorIdAsync ( string donorId )
        {
            return await _dbSet
                .FirstOrDefaultAsync( bt => bt.DonorId == donorId && !bt.IsDeleted );
        }

        public async Task<IEnumerable<BloodTest>> GetRecentTestsAsync ( int count )
        {
            return await _dbSet
                .Where( bt => !bt.IsDeleted )
                .Include( bt => bt.Donor )
                .OrderByDescending( bt => bt.TestDate )
                .Take( count )
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodTest>> GetPendingTestsAsync ()
        {
            return await _dbSet
                .Where( bt => !bt.IsDeleted && !bt.IsTestPassed )
                .Include( bt => bt.Donor )
                .OrderBy( bt => bt.TestDate )
                .ToListAsync();
        }
    }
}
