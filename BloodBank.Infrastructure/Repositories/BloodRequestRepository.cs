using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using BloodBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Repositories
{
    // BloodBank.Infrastructure/Repositories/BloodRequestRepository.cs
    public class BloodRequestRepository : GenericRepository<BloodRequest>, IBloodRequestRepository
    {
        public BloodRequestRepository ( BloodBankDbContext context ) : base( context )
        {
        }

        public async Task<IEnumerable<BloodRequest>> GetPendingRequestsAsync ()
        {
            return await _dbSet
                .Where( br => !br.IsDeleted && br.Status == RequestStatus.Pending )
                .Include( br => br.Hospital )
                .OrderByDescending( br => br.Priority )
                .ThenBy( br => br.RequiredDate )
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetHospitalRequestsAsync ( int hospitalId )
        {
            return await _dbSet
                .Where( br => !br.IsDeleted && br.HospitalId == hospitalId )
                .Include( br => br.AssignedUnits )
                .OrderByDescending( br => br.CreatedAt )
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetRequestsByBloodTypeAsync ( BloodType bloodType )
        {
            return await _dbSet
                .Where( br => !br.IsDeleted && br.BloodType == bloodType )
                .Include( br => br.Hospital )
                .OrderByDescending( br => br.Priority )
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetUrgentRequestsAsync ()
        {
            return await _dbSet
                .Where( br => !br.IsDeleted &&
                      ( br.Priority == RequestPriority.Urgent || br.Priority == RequestPriority.Emergency ) &&
                      br.Status == RequestStatus.Pending )
                .Include( br => br.Hospital )
                .OrderByDescending( br => br.Priority )
                .ToListAsync();
        }
    }
}
