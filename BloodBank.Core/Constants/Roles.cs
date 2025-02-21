using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Core.Constants
{
    public static class Roles
    {
        public const string Admin = "Admin";
        public const string Staff = "Staff";
        public const string Donor = "Donor";
        public const string Hospital = "Hospital";

        public static readonly IEnumerable<string> All = new [] { Admin, Staff, Donor, Hospital };
    }
}
