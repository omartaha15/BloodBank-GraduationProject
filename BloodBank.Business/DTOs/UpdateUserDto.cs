using BloodBank.Core.Enums;

namespace BloodBank.Business.DTOs
{
    public class UpdateUserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public BloodType BloodType { get; set; }
    }
}
