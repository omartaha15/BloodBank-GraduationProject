using BloodBank.Business.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodBank.Business.Interfaces
{
    public interface IBloodRequestService
    {
        Task<BloodRequestDto> CreateBloodRequestAsync ( CreateBloodRequestDto dto );
        Task<IEnumerable<BloodRequestDto>> GetAllBloodRequestsAsync ();
        Task ApproveBloodRequestAsync ( int requestId );
        Task RejectBloodRequestAsync ( int requestId );
    }
}
