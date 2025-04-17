using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodBank.Business.Interfaces
{
    public interface IBloodInventoryService
    {
        Task<List<BloodUnit>> GetAllAvailableUnitsAsync ();
        Task<int> GetAvailableUnitsCountByBloodTypeAsync ( BloodType bloodType );
        Task<Dictionary<BloodType, int>> GetBloodTypeStatsAsync ();
    }
}