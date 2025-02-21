using AutoMapper;
using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using OpenQA.Selenium;

namespace BloodBank.Business.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public AppointmentService (
            IAppointmentRepository appointmentRepository,
            IUserRepository userRepository,
            IMapper mapper )
        {
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<AppointmentDto> GetAppointmentByIdAsync ( int id )
        {
            var appointment = await _appointmentRepository.GetByIdAsync( id );
            if ( appointment == null )
                throw new NotFoundException( $"Appointment with ID {id} not found" );

            return _mapper.Map<AppointmentDto>( appointment );
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync ()
        {
            var appointments = await _appointmentRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<AppointmentDto>>( appointments );
        }

        public async Task<AppointmentDto> CreateAppointmentAsync ( CreateAppointmentDto appointmentDto )
        {
            // Validate donor exists
            var donor = await _userRepository.GetByIdAsync( appointmentDto.DonorId );
            if ( donor == null )
                throw new NotFoundException( $"Donor with ID {appointmentDto.DonorId} not found" );

            // Check if time slot is available
            if ( !await IsTimeSlotAvailableAsync( appointmentDto.AppointmentDate, appointmentDto.AppointmentTime ) )
                throw new NotFoundException( "The selected time slot is not available" );

            // Validate appointment date is in the future
            if ( appointmentDto.AppointmentDate.Date < DateTime.UtcNow.Date )
                throw new NotFoundException( "Appointment date must be in the future" );

            // Check if donor already has an appointment on the same day
            var existingAppointments = await _appointmentRepository.GetDonorAppointmentsAsync( appointmentDto.DonorId );
            if ( existingAppointments.Any( a => a.AppointmentDate.Date == appointmentDto.AppointmentDate.Date
                && a.Status == AppointmentStatus.Scheduled ) )
            {
                throw new NotFoundException( "Donor already has an appointment scheduled for this date" );
            }

            var appointment = _mapper.Map<Appointment>( appointmentDto );
            appointment.Status = AppointmentStatus.Scheduled;

            var result = await _appointmentRepository.AddAsync( appointment );
            return _mapper.Map<AppointmentDto>( result );
        }

        public async Task<IEnumerable<AppointmentDto>> GetUpcomingAppointmentsAsync ()
        {
            var appointments = await _appointmentRepository.GetUpcomingAppointmentsAsync( DateTime.UtcNow );
            return _mapper.Map<IEnumerable<AppointmentDto>>( appointments );
        }

        public async Task<IEnumerable<AppointmentDto>> GetDonorAppointmentsAsync ( string donorId )
        {
            // Validate donor exists
            var donor = await _userRepository.GetByIdAsync( donorId );
            if ( donor == null )
                throw new NotFoundException( $"Donor with ID {donorId} not found" );

            var appointments = await _appointmentRepository.GetDonorAppointmentsAsync( donorId );
            return _mapper.Map<IEnumerable<AppointmentDto>>( appointments );
        }

        public async Task UpdateAppointmentStatusAsync ( int id, AppointmentStatus status )
        {
            var appointment = await _appointmentRepository.GetByIdAsync( id );
            if ( appointment == null )
                throw new NotFoundException( $"Appointment with ID {id} not found" );

            // Validate status transition
            if ( !IsValidStatusTransition( appointment.Status, status ) )
                throw new NotFoundException( $"Invalid status transition from {appointment.Status} to {status}" );

            appointment.Status = status;
            await _appointmentRepository.UpdateAsync( appointment );
        }

        public async Task<bool> IsTimeSlotAvailableAsync ( DateTime date, TimeSpan time )
        {
            // Add business hours validation
            if ( !IsWithinBusinessHours( time ) )
                return false;

            return await _appointmentRepository.IsTimeSlotAvailableAsync( date, time );
        }

        public async Task CancelAppointmentAsync ( int id, string reason )
        {
            var appointment = await _appointmentRepository.GetByIdAsync( id );
            if ( appointment == null )
                throw new NotFoundException( $"Appointment with ID {id} not found" );

            // Can only cancel scheduled appointments
            if ( appointment.Status != AppointmentStatus.Scheduled )
                throw new NotFoundException( "Only scheduled appointments can be cancelled" );

            // Cannot cancel appointments less than 24 hours before
            if ( appointment.AppointmentDate < DateTime.UtcNow.AddHours( 24 ) )
                throw new NotFoundException( "Appointments cannot be cancelled less than 24 hours before the scheduled time" );

            appointment.Status = AppointmentStatus.Cancelled;
            appointment.Notes = reason;
            await _appointmentRepository.UpdateAsync( appointment );
        }

        public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByDateRangeAsync (
            DateTime startDate, DateTime endDate )
        {
            if ( startDate > endDate )
                throw new NotFoundException( "Start date must be before end date" );

            var appointments = await _appointmentRepository.GetAppointmentsByDateRangeAsync( startDate, endDate );
            return _mapper.Map<IEnumerable<AppointmentDto>>( appointments );
        }

        private bool IsWithinBusinessHours ( TimeSpan time )
        {
            // Example: Business hours 9 AM to 5 PM
            return time >= new TimeSpan( 9, 0, 0 ) && time <= new TimeSpan( 17, 0, 0 );
        }

        private bool IsValidStatusTransition ( AppointmentStatus currentStatus, AppointmentStatus newStatus )
        {
            switch ( currentStatus )
            {
                case AppointmentStatus.Scheduled:
                    return newStatus == AppointmentStatus.Completed ||
                           newStatus == AppointmentStatus.Cancelled ||
                           newStatus == AppointmentStatus.NoShow;

                case AppointmentStatus.Completed:
                case AppointmentStatus.Cancelled:
                case AppointmentStatus.NoShow:
                    return false; // These are final states

                default:
                    return false;
            }
        }
    }

}
