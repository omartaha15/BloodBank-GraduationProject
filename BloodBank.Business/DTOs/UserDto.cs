using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public Gender Gender { get; set; }
        public BloodType BloodType { get; set; }
        public IList<string> Roles { get; set; }
        public bool IsActive { get; set; }
    }

}
