using BloodBank.Core.Enums;
using System;

namespace BloodBank.Business.DTOs
{
    public class DonationDto
    {
        public int Id { get; set; }
        public string DonorId { get; set; }
        public string DonorName { get; set; }
        public DateTime DonationDate { get; set; }
        public BloodType BloodType { get; set; }
        public double Quantity { get; set; }
        public DonationStatus Status { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan AppointmentTime { get; set; }
        public string AppointmentNotes { get; set; }

        // New: The hospital chosen for the donation.
        public string HospitalId { get; set; }
        public string HospitalName { get; set; }

        public BloodUnitDto BloodUnit { get; set; }
    }
}
