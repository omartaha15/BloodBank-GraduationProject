using BloodBank.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Core.Interfaces
{
    // BloodBank.Core/Interfaces/IRepositories/IBloodTestRepository.cs
    public interface IBloodTestRepository : IGenericRepository<BloodTest>
    {
        Task<BloodTest> GetTestByDonationIdAsync ( int donationId );
        Task<IEnumerable<BloodTest>> GetRecentTestsAsync ( int count );
        Task<IEnumerable<BloodTest>> GetPendingTestsAsync ();
    }
}
