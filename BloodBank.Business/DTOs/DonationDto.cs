using BloodBank.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Business.DTOs
{
    public class DonationDto
    {
        public int Id { get; set; }
        public string DonorId { get; set; }
        public string DonorName { get; set; }
        public DateTime DonationDate { get; set; }
        public BloodType BloodType { get; set; }
        public double Quantity { get; set; }
        public DonationStatus Status { get; set; }
        public BloodTestDto BloodTest { get; set; }
        public BloodUnitDto BloodUnit { get; set; }  // Add this
    }

}
