using System.ComponentModel.DataAnnotations;

namespace BloodBank.Business.DTOs
{
    public class ChangePasswordDto
    {
        [Required( ErrorMessage = "Current password is required" )]
        public string CurrentPassword { get; set; }

        [Required( ErrorMessage = "New password is required" )]
        [MinLength( 6, ErrorMessage = "Password must be at least 6 characters" )]
        public string NewPassword { get; set; }

        [Required( ErrorMessage = "Confirm password is required" )]
        [Compare( "NewPassword", ErrorMessage = "Passwords do not match" )]
        public string ConfirmNewPassword { get; set; }
    }

}
