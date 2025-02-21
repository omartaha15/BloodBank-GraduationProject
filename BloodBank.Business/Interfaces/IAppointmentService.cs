using BloodBank.Business.DTOs;
using BloodBank.Core.Enums;

namespace BloodBank.Business.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> GetAppointmentByIdAsync ( int id );
        Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync ();
        Task<AppointmentDto> CreateAppointmentAsync ( CreateAppointmentDto appointmentDto );
        Task<IEnumerable<AppointmentDto>> GetUpcomingAppointmentsAsync ();
        Task<IEnumerable<AppointmentDto>> GetDonorAppointmentsAsync ( string donorId );
        Task UpdateAppointmentStatusAsync ( int id, AppointmentStatus status );
        Task<bool> IsTimeSlotAvailableAsync ( DateTime date, TimeSpan time );
        Task CancelAppointmentAsync ( int id, string reason );
        Task<IEnumerable<AppointmentDto>> GetAppointmentsByDateRangeAsync ( DateTime startDate, DateTime endDate );
    }
}
