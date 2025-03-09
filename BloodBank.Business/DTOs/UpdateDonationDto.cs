using BloodBank.Core.Enums;
using System;

namespace BloodBank.Business.DTOs
{
    public class UpdateDonationDto
    {
        public DonationStatus Status { get; set; }
        // Allow updating appointment details along with status if needed
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentNotes { get; set; }
    }
}
