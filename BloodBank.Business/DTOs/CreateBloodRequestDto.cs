using BloodBank.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace BloodBank.Business.DTOs
{
    public class CreateBloodRequestDto
    {
        public int HospitalId { get; set; }

        [Required( ErrorMessage = "Blood type is required" )]
        public BloodType BloodType { get; set; }

        [Required( ErrorMessage = "Quantity is required" )]
        [Range( 1, 10000, ErrorMessage = "Quantity must be between 1 and 10000 ml" )]
        public double QuantityRequired { get; set; }

        [Required( ErrorMessage = "Priority is required" )]
        public RequestPriority Priority { get; set; }

        [Required( ErrorMessage = "Required date is required" )]
        //[FutureDate( ErrorMessage = "Required date must be in the future" )]
        public DateTime RequiredDate { get; set; }

        public string Notes { get; set; }
    }
}
