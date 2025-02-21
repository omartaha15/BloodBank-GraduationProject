using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }
        public string DonorId { get; set; }
        public string DonorName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Notes { get; set; }
    }
}
