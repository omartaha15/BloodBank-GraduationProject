using AutoMapper;
using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BloodBank.Business.Services
{
    public class DonationService : IDonationService
    {
        private readonly IDonationRepository _donationRepository;
        private readonly IMapper _mapper;

        public DonationService ( IDonationRepository donationRepository, IMapper mapper )
        {
            _donationRepository = donationRepository;
            _mapper = mapper;
        }

        public async Task<DonationDto> GetDonationByIdAsync ( int id )
        {
            var donation = await _donationRepository.GetByIdAsync( id );
            if ( donation == null )
                throw new NotFoundException( $"Donation with ID {id} not found" );

            return _mapper.Map<DonationDto>( donation );
        }

        public async Task<IEnumerable<DonationDto>> GetAllDonationsAsync ()
        {
            var donations = await _donationRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<DonationDto>>( donations );
        }

        public async Task<DonationDto> CreateDonationAsync ( CreateDonationDto donationDto )
        {
            var donation = _mapper.Map<Donation>( donationDto );
            donation.Status = DonationStatus.Pending;
            donation.DonationDate = DateTime.UtcNow;

            var result = await _donationRepository.AddAsync( donation );
            return _mapper.Map<DonationDto>( result );
        }

        public async Task UpdateDonationStatusAsync ( int id, DonationStatus status )
        {
            var donation = await _donationRepository.GetByIdAsync( id );
            if ( donation == null )
                throw new NotFoundException( $"Donation with ID {id} not found" );

            donation.Status = status;
            await _donationRepository.UpdateAsync( donation );
        }

        public async Task<IEnumerable<DonationDto>> GetDonationsByDonorAsync ( string donorId )
        {
            var donations = await _donationRepository.GetDonationsByDonorIdAsync( donorId );
            return _mapper.Map<IEnumerable<DonationDto>>( donations );
        }

        public async Task<IEnumerable<DonationDto>> GetDonationsByBloodTypeAsync ( BloodType bloodType )
        {
            var donations = await _donationRepository.GetDonationsByBloodTypeAsync( bloodType );
            return _mapper.Map<IEnumerable<DonationDto>>( donations );
        }

        public async Task<IEnumerable<DonationDto>> GetRecentDonationsAsync ( int count )
        {
            var donations = await _donationRepository.GetRecentDonationsAsync( count );
            return _mapper.Map<IEnumerable<DonationDto>>( donations );
        }
    }
}
