using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Core.Interfaces
{
    public interface IBloodUnitRepository : IGenericRepository<BloodUnit>
    {
        Task<IEnumerable<BloodUnit>> GetAvailableUnitsByBloodTypeAsync ( BloodType bloodType );
        Task<int> GetAvailableUnitsCountByBloodTypeAsync ( BloodType bloodType );
        Task<IEnumerable<BloodUnit>> GetExpiringUnitsAsync ( int daysThreshold );
        Task<BloodUnit> GetUnitByBarcodeAsync ( string unitNumber );
        Task<IEnumerable<BloodUnit>> GetUnitsByDonationIdAsync ( int donationId );
    }
}
