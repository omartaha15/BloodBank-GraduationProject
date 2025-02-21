using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class UpdateAppointmentDto
    {
        public AppointmentStatus Status { get; set; }
        public string Notes { get; set; }
    }
}
