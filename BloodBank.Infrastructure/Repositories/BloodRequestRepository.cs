using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using BloodBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Repositories
{
    public class BloodRequestRepository : IBloodRequestRepository
    {
        private readonly BloodBankDbContext _context;

        public BloodRequestRepository(BloodBankDbContext context)
        {
            _context = context;
        }

        public async Task<BloodRequest> GetByIdAsync(int id)
        {
            return await _context.BloodRequests
                .Include(br => br.Hospital)
                .Include(br => br.AssignedUnits)
                .FirstOrDefaultAsync(br => br.Id == id && !br.IsDeleted);
        }

        public async Task<IEnumerable<BloodRequest>> GetAllByHospitalIdAsync(string hospitalId)
        {
            return await _context.BloodRequests
                .Include(br => br.Hospital)
                .Where(br => br.HospitalId == hospitalId && !br.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<BloodRequest>> GetPendingRequestsAsync()
        {
            return await _context.BloodRequests
                .Include(br => br.Hospital)
                .Where(br => br.Status == RequestStatus.Pending && !br.IsDeleted)
                .ToListAsync();
        }

        public async Task AddAsync(BloodRequest bloodRequest)
        {
            await _context.BloodRequests.AddAsync(bloodRequest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BloodRequest bloodRequest)
        {
            _context.BloodRequests.Update(bloodRequest);
            await _context.SaveChangesAsync();
        }
    }
}
