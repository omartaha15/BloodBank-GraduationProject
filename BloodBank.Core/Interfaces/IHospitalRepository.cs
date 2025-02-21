using BloodBank.Core.Entities;

namespace BloodBank.Core.Interfaces
{
    // BloodBank.Core/Interfaces/IRepositories/IHospitalRepository.cs
    public interface IHospitalRepository : IGenericRepository<Hospital>
    {
        Task<IEnumerable<Hospital>> GetActiveHospitalsAsync ();
        Task<Hospital> GetHospitalWithRequestsAsync ( int hospitalId );
        Task<Hospital> GetByEmailAsync ( string email );
        Task<Hospital> GetByContactNumberAsync ( string contactNumber );
    }
}
