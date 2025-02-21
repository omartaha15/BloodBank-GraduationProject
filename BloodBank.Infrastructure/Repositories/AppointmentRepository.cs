using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using BloodBank.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BloodBank.Infrastructure.Repositories
{
    public class AppointmentRepository : GenericRepository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository ( BloodBankDbContext context ) : base( context )
        {
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync ( DateTime date )
        {
            return await _dbSet
                .Where( a => a.AppointmentDate >= date &&
                           !a.IsDeleted &&
                           a.Status == AppointmentStatus.Scheduled )
                .Include( a => a.Donor )
                .OrderBy( a => a.AppointmentDate )
                .ThenBy( a => a.AppointmentTime )
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetDonorAppointmentsAsync ( string donorId )
        {
            return await _dbSet
                .Where( a => a.DonorId == donorId && !a.IsDeleted )
                .OrderByDescending( a => a.AppointmentDate )
                .ThenBy( a => a.AppointmentTime )
                .ToListAsync();
        }

        public async Task<bool> IsTimeSlotAvailableAsync ( DateTime date, TimeSpan time )
        {
            return !await _dbSet
                .AnyAsync( a => a.AppointmentDate.Date == date.Date &&
                              a.AppointmentTime == time &&
                              a.Status == AppointmentStatus.Scheduled &&
                              !a.IsDeleted );
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync (
            DateTime startDate, DateTime endDate )
        {
            return await _dbSet
                .Where( a => a.AppointmentDate >= startDate &&
                           a.AppointmentDate <= endDate &&
                           !a.IsDeleted )
                .Include( a => a.Donor )
                .OrderBy( a => a.AppointmentDate )
                .ThenBy( a => a.AppointmentTime )
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync ( AppointmentStatus status )
        {
            return await _dbSet
                .Where( a => a.Status == status && !a.IsDeleted )
                .Include( a => a.Donor )
                .OrderBy( a => a.AppointmentDate )
                .ThenBy( a => a.AppointmentTime )
                .ToListAsync();
        }
    }
}
