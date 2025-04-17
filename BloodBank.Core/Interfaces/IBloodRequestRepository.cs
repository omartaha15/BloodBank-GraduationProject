using BloodBank.Core.Entities;
using BloodBank.Core.Enums;

namespace BloodBank.Core.Interfaces
{
    public interface IBloodRequestRepository
    {
        Task<BloodRequest> GetByIdAsync ( int id );
        Task<IEnumerable<BloodRequest>> GetAllByHospitalIdAsync ( string hospitalId );
        Task<IEnumerable<BloodRequest>> GetPendingRequestsAsync ();
        Task AddAsync ( BloodRequest bloodRequest );
        Task UpdateAsync ( BloodRequest bloodRequest );
    }
}
