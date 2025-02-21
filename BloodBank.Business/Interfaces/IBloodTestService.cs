using BloodBank.Business.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Business.Interfaces
{
    public interface IBloodTestService
    {
        Task<BloodTestDto> GetTestByIdAsync ( int id );
        Task<BloodTestDto> CreateBloodTestAsync ( CreateBloodTestDto testDto );
        Task<BloodTestDto> UpdateTestResultsAsync ( int id, UpdateBloodTestDto testDto );
        Task<BloodTestDto> GetTestByDonationIdAsync ( int donationId );
        Task<IEnumerable<BloodTestDto>> GetPendingTestsAsync ();
        Task<bool> ValidateTestResultsAsync ( int testId );
    }
}
