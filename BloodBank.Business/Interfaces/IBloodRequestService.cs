using BloodBank.Business.DTOs;
using BloodBank.Core.Enums;

namespace BloodBank.Business.Interfaces
{
    public interface IBloodRequestService
    {
        Task<BloodRequestDto> GetRequestByIdAsync ( int id );
        Task<IEnumerable<BloodRequestDto>> GetAllRequestsAsync ();
        Task<BloodRequestDto> CreateRequestAsync ( CreateBloodRequestDto requestDto );
        Task UpdateRequestStatusAsync ( int id, RequestStatus status );
        Task<IEnumerable<BloodRequestDto>> GetPendingRequestsAsync ();
        Task<IEnumerable<BloodRequestDto>> GetHospitalRequestsAsync ( int hospitalId );
        Task<IEnumerable<BloodRequestDto>> GetRequestsByBloodTypeAsync ( BloodType bloodType );
        Task<IEnumerable<BloodRequestDto>> GetUrgentRequestsAsync ();
        Task AssignBloodUnitsToRequestAsync ( int requestId, List<int> bloodUnitIds );
    }
}
