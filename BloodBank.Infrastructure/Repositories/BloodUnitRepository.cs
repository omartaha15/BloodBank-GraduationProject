using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
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
    public class BloodUnitRepository : GenericRepository<BloodUnit>, IBloodUnitRepository
    {
        public BloodUnitRepository ( BloodBankDbContext context ) : base( context )
        {
        }

        public async Task<IEnumerable<BloodUnit>> GetAvailableUnitsByBloodTypeAsync ( BloodType bloodType )
        {
            return await _dbSet
                .Where( bu => bu.BloodType == bloodType &&
                            bu.Status == BloodUnitStatus.Available &&
                            !bu.IsDeleted &&
                            bu.ExpiryDate > DateTime.UtcNow )
                .Include( bu => bu.Donation )
                .OrderBy( bu => bu.ExpiryDate )
                .ToListAsync();
        }

        public async Task<int> GetAvailableUnitsCountByBloodTypeAsync ( BloodType bloodType )
        {
            return await _dbSet
                .CountAsync( bu => bu.BloodType == bloodType &&
                                bu.Status == BloodUnitStatus.Available &&
                                !bu.IsDeleted &&
                                bu.ExpiryDate > DateTime.UtcNow );
        }

        public async Task<IEnumerable<BloodUnit>> GetExpiringUnitsAsync ( int daysThreshold )
        {
            var thresholdDate = DateTime.UtcNow.AddDays( daysThreshold );
            return await _dbSet
                .Where( bu => bu.Status == BloodUnitStatus.Available &&
                            !bu.IsDeleted &&
                            bu.ExpiryDate <= thresholdDate )
                .Include( bu => bu.Donation )
                .OrderBy( bu => bu.ExpiryDate )
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodUnit>> GetUnitsByStatusAsync ( BloodUnitStatus status )
        {
            return await _dbSet
                .Where( bu => bu.Status == status && !bu.IsDeleted )
                .Include( bu => bu.Donation )
                .OrderBy( bu => bu.ExpiryDate )
                .ToListAsync();
        }

        public async Task<BloodUnit> GetUnitByBarcodeAsync ( string unitNumber )
        {
            return await _dbSet
                .FirstOrDefaultAsync( bu => bu.UnitNumber == unitNumber && !bu.IsDeleted );
        }

        public async Task<IEnumerable<BloodUnit>> GetUnitsByDonationIdAsync ( int donationId )
        {
            return await _dbSet
                .Where( bu => bu.DonationId == donationId && !bu.IsDeleted )
                .ToListAsync();
        }
    }
}
