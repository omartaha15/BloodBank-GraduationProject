using BloodBank.Core.Enums;

namespace BloodBank.Core.Entities
{
    public class Appointment : BaseEntity
    {
        public string DonorId { get; set; }
        public virtual User Donor { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public AppointmentStatus Status { get; set; }
        public string Notes { get; set; }
    }
}
