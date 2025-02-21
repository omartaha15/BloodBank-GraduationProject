using BloodBank.Business.DTOs;
using BloodBank.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Business.Interfaces
{
    public interface IDonationService
    {
        Task<DonationDto> GetDonationByIdAsync ( int id );
        Task<IEnumerable<DonationDto>> GetAllDonationsAsync ();
        Task<DonationDto> CreateDonationAsync ( CreateDonationDto donationDto );
        Task UpdateDonationStatusAsync ( int id, DonationStatus status );
        Task<IEnumerable<DonationDto>> GetDonationsByDonorAsync ( string donorId );
        Task<IEnumerable<DonationDto>> GetDonationsByBloodTypeAsync ( BloodType bloodType );
        Task<IEnumerable<DonationDto>> GetRecentDonationsAsync ( int count );
    }
}
