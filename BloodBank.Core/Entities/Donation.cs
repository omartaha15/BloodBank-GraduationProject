using BloodBank.Core.Enums;

namespace BloodBank.Core.Entities
{
    public class Donation : BaseEntity
    {
        public string DonorId { get; set; }
        public virtual User Donor { get; set; }

        public DateTime DonationDate { get; set; }
        public BloodType BloodType { get; set; }
        public double Quantity { get; set; } // in milliliters
        public DonationStatus Status { get; set; }

        // Merged Appointment details:
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentNotes { get; set; }

        // New: Hospital chosen for this donation.
        public string HospitalId { get; set; }
        public virtual User Hospital { get; set; }

        // Association with blood unit.
        public virtual BloodUnit BloodUnit { get; set; }
    }
}
