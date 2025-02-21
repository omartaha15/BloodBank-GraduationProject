namespace BloodBank.Business.DTOs
{
    // BloodBank.Business/DTOs/Admin/ChangeRoleDto.cs
    public class ChangeRoleDto
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public IList<string> CurrentRoles { get; set; }
        public List<string> AvailableRoles { get; set; }
        public string NewRole { get; set; }
    }

}
