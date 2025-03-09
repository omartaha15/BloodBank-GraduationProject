using BloodBank.Business.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodBank.Business.Interfaces
{
    public interface IBloodTestService
    {
        Task<BloodTestDto> GetTestByIdAsync ( int id );
        Task<BloodTestDto> CreateBloodTestAsync ( CreateBloodTestDto testDto );
        Task<BloodTestDto> UpdateTestResultsAsync ( int id, UpdateBloodTestDto testDto );
        Task<BloodTestDto> GetTestByDonorIdAsync ( string donorId );
        Task<IEnumerable<BloodTestDto>> GetPendingTestsAsync ();
        Task<bool> ValidateTestResultsAsync ( int testId );

        // New methods for hospital approval
        Task ApproveBloodTestAsync ( int id );
        Task RejectBloodTestAsync ( int id );
    }
}
