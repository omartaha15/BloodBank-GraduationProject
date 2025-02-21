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
    public class BloodTestService : IBloodTestService
    {
        private readonly IBloodTestRepository _bloodTestRepository;
        private readonly IDonationRepository _donationRepository;
        private readonly IMapper _mapper;

        public BloodTestService (
            IBloodTestRepository bloodTestRepository,
            IDonationRepository donationRepository,
            IMapper mapper )
        {
            _bloodTestRepository = bloodTestRepository;
            _donationRepository = donationRepository;
            _mapper = mapper;
        }

        public async Task<BloodTestDto> GetTestByIdAsync ( int id )
        {
            var test = await _bloodTestRepository.GetByIdAsync( id );
            if ( test == null )
                throw new NotFoundException( $"Blood test with ID {id} not found" );

            return _mapper.Map<BloodTestDto>( test );
        }

        public async Task<BloodTestDto> CreateBloodTestAsync ( CreateBloodTestDto testDto )
        {
            // Validate if donation exists
            var donation = await _donationRepository.GetByIdAsync( testDto.DonationId );
            if ( donation == null )
                throw new NotFoundException( $"Donation with ID {testDto.DonationId} not found" );

            var bloodTest = _mapper.Map<BloodTest>( testDto );
            bloodTest.TestDate = DateTime.UtcNow;

            // Determine if test is passed based on all test results
            bloodTest.IsTestPassed = !( testDto.HivTest || testDto.HepatitisB ||
                                     testDto.HepatitisC || testDto.Syphilis ||
                                     testDto.Malaria );

            var result = await _bloodTestRepository.AddAsync( bloodTest );

            // Update donation status based on test results
            donation.Status = bloodTest.IsTestPassed ?
                DonationStatus.Approved : DonationStatus.Rejected;
            await _donationRepository.UpdateAsync( donation );

            return _mapper.Map<BloodTestDto>( result );
        }

        public async Task<BloodTestDto> UpdateTestResultsAsync ( int id, UpdateBloodTestDto testDto )
        {
            var bloodTest = await _bloodTestRepository.GetByIdAsync( id );
            if ( bloodTest == null )
                throw new NotFoundException( $"Blood test with ID {id} not found" );

            _mapper.Map( testDto, bloodTest );
            bloodTest.TestDate = DateTime.UtcNow;

            await _bloodTestRepository.UpdateAsync( bloodTest );

            // Update related donation status if needed
            var donation = await _donationRepository.GetByIdAsync( bloodTest.DonationId );
            if ( donation != null )
            {
                donation.Status = bloodTest.IsTestPassed ?
                    DonationStatus.Approved : DonationStatus.Rejected;
                await _donationRepository.UpdateAsync( donation );
            }

            return _mapper.Map<BloodTestDto>( bloodTest );
        }

        public async Task<BloodTestDto> GetTestByDonationIdAsync ( int donationId )
        {
            var test = await _bloodTestRepository.GetTestByDonationIdAsync( donationId );
            if ( test == null )
                throw new NotFoundException( $"Blood test for donation ID {donationId} not found" );

            return _mapper.Map<BloodTestDto>( test );
        }

        public async Task<IEnumerable<BloodTestDto>> GetPendingTestsAsync ()
        {
            var tests = await _bloodTestRepository.GetPendingTestsAsync();
            return _mapper.Map<IEnumerable<BloodTestDto>>( tests );
        }

        public async Task<bool> ValidateTestResultsAsync ( int testId )
        {
            var test = await _bloodTestRepository.GetByIdAsync( testId );
            if ( test == null )
                throw new NotFoundException( $"Blood test with ID {testId} not found" );

            // Implement validation logic
            bool isValid = !( test.HivTest || test.HepatitisB ||
                            test.HepatitisC || test.Syphilis ||
                            test.Malaria );

            if ( test.IsTestPassed != isValid )
            {
                test.IsTestPassed = isValid;
                await _bloodTestRepository.UpdateAsync( test );

                // Update donation status
                var donation = await _donationRepository.GetByIdAsync( test.DonationId );
                if ( donation != null )
                {
                    donation.Status = isValid ?
                        DonationStatus.Approved : DonationStatus.Rejected;
                    await _donationRepository.UpdateAsync( donation );
                }
            }

            return isValid;
        }
    }
}
