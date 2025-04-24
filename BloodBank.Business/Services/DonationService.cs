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
using System.Threading.Tasks;

namespace BloodBank.Business.Services
{
    public class DonationService : IDonationService
    {
        private readonly IDonationRepository _donationRepository;
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;
        private readonly IBloodTestService _bloodTestService;

        public DonationService (
            IDonationRepository donationRepository,
            IMapper mapper,
            IUserRepository userRepository,
            IBloodTestService bloodTestService )
        {
            _donationRepository = donationRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _bloodTestService = bloodTestService;
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
            var donations = await _donationRepository.GetAllDonationsAsync();
            return _mapper.Map<IEnumerable<DonationDto>>( donations );
        }

        public async Task<DonationDto> CreateDonationAsync ( CreateDonationDto donationDto )
        {
            // Get donor details.
            var donor = await _userRepository.GetByIdAsync( donationDto.DonorId );
            if ( donor == null )
                throw new NotFoundException( $"Donor with ID {donationDto.DonorId} not found." );

            // Verify the donor has a blood test.
            var bloodTest = await _bloodTestService.GetTestByDonorIdAsync( donationDto.DonorId );
            if ( bloodTest == null )
                throw new InvalidOperationException( "You must complete a blood test before donating." );

            // Ensure the blood test has passed and is approved by a hospital.
            if ( !bloodTest.IsTestPassed || bloodTest.HospitalApprovalStatus != HospitalApprovalStatus.Approved )
            {
                throw new InvalidOperationException( "Your blood test is not approved yet. Please wait for hospital approval before making a donation." );
            }

            // Enforce the 3‑month donation rule.
            var donorDonations = await _donationRepository.GetDonationsByDonorIdAsync( donationDto.DonorId );
            var lastDonation = donorDonations.OrderByDescending( d => d.DonationDate ).FirstOrDefault();
            if ( lastDonation != null && lastDonation.DonationDate.AddMonths( 3 ) > DateTime.UtcNow )
            {
                throw new InvalidOperationException( "You cannot donate more than once within a 3‑month period." );
            }

            // Map DTO to Donation entity.
            var donation = _mapper.Map<Donation>( donationDto );
            donation.Status = DonationStatus.Pending;
            donation.DonationDate = DateTime.UtcNow;
            // Appointment details and HospitalId are mapped from donationDto.

            var result = await _donationRepository.AddAsync( donation );
            return _mapper.Map<DonationDto>( result );
        }

        /// <summary>
        /// Updates donation status and appointment details using the UpdateDonationDto.
        /// </summary>
        public async Task UpdateDonationDetailsAsync ( int id, UpdateDonationDto updateDto )
        {
            var donation = await _donationRepository.GetByIdAsync( id );
            if ( donation == null )
                throw new NotFoundException( $"Donation with ID {id} not found." );

            // Update donation details from updateDto.
            donation.Status = updateDto.Status;
            donation.AppointmentDate = updateDto.AppointmentDate;
            donation.AppointmentTime = updateDto.AppointmentTime;
            donation.AppointmentNotes = updateDto.AppointmentNotes;
            donation.BloodType = updateDto.BloodType;
            donation.Quantity = updateDto.Quantity;

            await _donationRepository.UpdateAsync( donation );
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

        public async Task<IEnumerable<DonationDto>> GetDonationsByHospitalAsync ( string hospitalId )
        {
            // Retrieve all donations; optionally, you may want to query the repository directly.
            var allDonations = await _donationRepository.GetAllDonationsAsync();

            // Filter donations where the HospitalId matches.
            var hospitalDonations = allDonations.Where( d => d.HospitalId == hospitalId );

            return _mapper.Map<IEnumerable<DonationDto>>( hospitalDonations );
        }
    }
}
