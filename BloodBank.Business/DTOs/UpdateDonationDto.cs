using BloodBank.Core.Enums;
using System;

namespace BloodBank.Business.DTOs
{
    public class UpdateDonationDto
    {
        public DonationStatus Status { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentNotes { get; set; }
        public BloodType BloodType { get; set; }
        public double Quantity { get; set; }
    }
}
