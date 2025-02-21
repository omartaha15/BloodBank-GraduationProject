using System.ComponentModel.DataAnnotations;

namespace BloodBank.Business.DTOs
{
    public class UpdateHospitalDto
    {
        [Required( ErrorMessage = "Name is required" )]
        [StringLength( 100, ErrorMessage = "Name cannot be longer than 100 characters" )]
        public string Name { get; set; }

        [Required( ErrorMessage = "Address is required" )]
        public string Address { get; set; }

        [Required( ErrorMessage = "Contact person is required" )]
        public string ContactPerson { get; set; }

        [Required( ErrorMessage = "Contact number is required" )]
        [Phone( ErrorMessage = "Invalid phone number" )]
        public string ContactNumber { get; set; }

        [Required( ErrorMessage = "Email is required" )]
        [EmailAddress( ErrorMessage = "Invalid email address" )]
        public string Email { get; set; }
    }
}
