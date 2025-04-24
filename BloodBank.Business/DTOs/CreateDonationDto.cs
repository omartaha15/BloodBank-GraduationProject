using BloodBank.Core.Enums;
using System;

namespace BloodBank.Business.DTOs
{
    public class CreateDonationDto
    {
        public string DonorId { get; set; }
        public BloodType BloodType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // computed on the fly:
        public string DonorName => $"{FirstName} {LastName}";

        public double Quantity { get; set; }
        public DateTime DonationDate { get; set; }

        // Merged Appointment details:
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentNotes { get; set; }

        // New: The hospital chosen for the donation.
        public string HospitalId { get; set; }
    }
}
