using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Core.Interfaces
{
    public interface IDonationRepository : IGenericRepository<Donation>
    {
        Task<IEnumerable<Donation>> GetDonationsByDonorIdAsync ( string donorId );
        Task<IEnumerable<Donation>> GetDonationsByBloodTypeAsync ( BloodType bloodType );
        Task<IEnumerable<Donation>> GetRecentDonationsAsync ( int count );
    }
}
