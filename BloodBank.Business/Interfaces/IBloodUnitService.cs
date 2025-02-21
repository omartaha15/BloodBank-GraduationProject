using BloodBank.Business.DTOs;
using BloodBank.Core.Enums;

namespace BloodBank.Business.Interfaces
{
    public interface IBloodUnitService
    {
        Task<BloodUnitDto> GetBloodUnitByIdAsync ( int id );
        Task<BloodUnitDto> CreateBloodUnitAsync ( CreateBloodUnitDto unitDto );
        Task UpdateUnitStatusAsync ( int id, BloodUnitStatus status );
        Task<IEnumerable<BloodUnitDto>> GetAvailableUnitsByBloodTypeAsync ( BloodType bloodType );
        Task<IEnumerable<BloodUnitDto>> GetExpiringUnitsAsync ( int daysThreshold );
        Task<BloodUnitDto> GetUnitByBarcodeAsync ( string barcode );
        Task<int> GetAvailableUnitsCountByBloodTypeAsync ( BloodType bloodType );
        Task<IEnumerable<BloodUnitDto>> GetUnitsByDonationIdAsync ( int donationId );
    }
}
