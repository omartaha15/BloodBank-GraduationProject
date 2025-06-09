using BloodBank.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodBank.Core.Interfaces
{
    public interface IBloodTestRepository : IGenericRepository<BloodTest>
    {
        // Since each donor can have only one blood test, retrieve the test by donor ID.
        Task<BloodTest> GetTestByDonorIdAsync ( string donorId );
        Task<IEnumerable<BloodTest>> GetRecentTestsAsync ( int count );
        Task<IEnumerable<BloodTest>> GetPendingTestsAsync ();
    }
}
