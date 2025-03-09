using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BloodBank.Business.DTOs
{
    public class ChangeRoleDto
    {
        public string UserId { get; set; }

        [BindNever]
        public string UserEmail { get; set; }

        [BindNever]
        public IList<string> CurrentRoles { get; set; }

        [BindNever]
        public List<string> AvailableRoles { get; set; }

        [Required( ErrorMessage = "Please select a new role." )]
        public string NewRole { get; set; }
    }
}
