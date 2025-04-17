using BloodBank.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodBank.Application.Services
{
    public interface IBloodRequestService
    {
        Task CreateBloodRequestAsync ( BloodRequest bloodRequest, string hospitalId );
        Task<IEnumerable<BloodRequest>> GetHospitalRequestsAsync ( string hospitalId );
        Task<IEnumerable<BloodRequest>> GetPendingRequestsAsync ();
        Task ApproveBloodRequestAsync ( int requestId, string staffId );
        Task ProcessBloodRequestAsync ( int requestId, string staffId );
        Task RejectBloodRequestAsync ( int requestId, string staffId, string rejectionReason );
    }
}