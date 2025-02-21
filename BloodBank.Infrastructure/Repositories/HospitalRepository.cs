using BloodBank.Core.Entities;
using BloodBank.Core.Interfaces;
using BloodBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Repositories
{
    // BloodBank.Infrastructure/Repositories/HospitalRepository.cs
    public class HospitalRepository : GenericRepository<Hospital>, IHospitalRepository
    {
        public HospitalRepository ( BloodBankDbContext context ) : base( context )
        {
        }

        public async Task<IEnumerable<Hospital>> GetActiveHospitalsAsync ()
        {
            return await _dbSet
                .Where( h => !h.IsDeleted )
                .OrderBy( h => h.Name )
                .ToListAsync();
        }

        public async Task<Hospital> GetHospitalWithRequestsAsync ( int hospitalId )
        {
            return await _dbSet
                .Include( h => h.BloodRequests )
                .FirstOrDefaultAsync( h => h.Id == hospitalId && !h.IsDeleted );
        }

        public async Task<Hospital> GetByEmailAsync ( string email )
        {
            return await _dbSet
                .FirstOrDefaultAsync( h => h.Email == email && !h.IsDeleted );
        }

        public async Task<Hospital> GetByContactNumberAsync ( string contactNumber )
        {
            return await _dbSet
                .FirstOrDefaultAsync( h => h.ContactNumber == contactNumber && !h.IsDeleted );
        }
    }
}
