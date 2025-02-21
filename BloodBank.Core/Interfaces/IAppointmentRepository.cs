using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Core.Interfaces
{
    public interface IAppointmentRepository : IGenericRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync ( DateTime date );
        Task<IEnumerable<Appointment>> GetDonorAppointmentsAsync ( string donorId );
        Task<bool> IsTimeSlotAvailableAsync ( DateTime date, TimeSpan time );
        Task<IEnumerable<Appointment>> GetAppointmentsByDateRangeAsync ( DateTime startDate, DateTime endDate );
        Task<IEnumerable<Appointment>> GetAppointmentsByStatusAsync ( AppointmentStatus status );
    }
}
