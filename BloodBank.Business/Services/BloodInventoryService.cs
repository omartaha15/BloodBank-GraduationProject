using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using BloodBank.Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodBank.Business.Services
{
    public class BloodInventoryService : IBloodInventoryService
    {
        private readonly IBloodUnitRepository _bloodUnitRepository;

        public BloodInventoryService ( IBloodUnitRepository bloodUnitRepository )
        {
            _bloodUnitRepository = bloodUnitRepository;
        }

        public async Task<List<BloodUnit>> GetAllAvailableUnitsAsync ()
        {
            var units = await _bloodUnitRepository.GetAllAsync();
            return units.Where( u => u.Status == BloodUnitStatus.Available ).ToList();
        }

        public async Task<int> GetAvailableUnitsCountByBloodTypeAsync ( BloodType bloodType )
        {
            return await _bloodUnitRepository.GetAvailableUnitsCountByBloodTypeAsync( bloodType );
        }

        public async Task<Dictionary<BloodType, int>> GetBloodTypeStatsAsync ()
        {
            var units = await _bloodUnitRepository.GetAllAsync();
            var availableUnits = units.Where( u => u.Status == BloodUnitStatus.Available );

            var stats = new Dictionary<BloodType, int>();
            foreach ( BloodType bloodType in Enum.GetValues( typeof( BloodType ) ) )
            {
                var count = availableUnits.Count( u => u.BloodType == bloodType );
                stats [ bloodType ] = count;
            }

            return stats;
        }
    }
}