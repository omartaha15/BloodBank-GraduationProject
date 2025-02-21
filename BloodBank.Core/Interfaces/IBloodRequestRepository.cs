using BloodBank.Core.Entities;
using BloodBank.Core.Enums;

namespace BloodBank.Core.Interfaces
{
    // BloodBank.Core/Interfaces/IRepositories/IBloodRequestRepository.cs
    public interface IBloodRequestRepository : IGenericRepository<BloodRequest>
    {
        Task<IEnumerable<BloodRequest>> GetPendingRequestsAsync ();
        Task<IEnumerable<BloodRequest>> GetHospitalRequestsAsync ( int hospitalId );
        Task<IEnumerable<BloodRequest>> GetRequestsByBloodTypeAsync ( BloodType bloodType );
        Task<IEnumerable<BloodRequest>> GetUrgentRequestsAsync ();
    }
}
