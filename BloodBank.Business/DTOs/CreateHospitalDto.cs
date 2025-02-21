using System.ComponentModel.DataAnnotations;

namespace BloodBank.Business.DTOs
{
    public class CreateHospitalDto
    {
        [Required( ErrorMessage = "Name is required" )]
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

        // Optional: Add password fields if you want admin to set initial password
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
