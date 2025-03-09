namespace BloodBank.Business.DTOs
{
    // BloodBank.Business/DTOs/Admin/UserManagementDto.cs
    public class UserManagementDto
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public IList<string> Roles { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }



}
