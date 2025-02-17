using BloodBank.Core.Enums;
using Microsoft.AspNetCore.Identity;

namespace BloodBank.Core.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }
        public BloodType BloodType { get; set; }
        public virtual ICollection<Donation> Donations { get; set; }
        public virtual ICollection<Appointment> Appointments { get; set; }
    }
}
