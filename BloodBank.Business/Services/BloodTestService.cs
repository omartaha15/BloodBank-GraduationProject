﻿using AutoMapper;
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
    public class BloodTestService : IBloodTestService
    {
        private readonly IBloodTestRepository _bloodTestRepository;
        private readonly IDonationRepository _donationRepository;
        private readonly IMapper _mapper;

        public BloodTestService ( IBloodTestRepository bloodTestRepository, IDonationRepository donationRepository, IMapper mapper )
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
            var existingTest = await _bloodTestRepository.GetTestByDonorIdAsync( testDto.DonorId );
            if ( existingTest != null )
                throw new InvalidOperationException( $"A blood test for donor ID {testDto.DonorId} already exists." );

            var bloodTest = _mapper.Map<BloodTest>( testDto );
            bloodTest.TestDate = DateTime.UtcNow;
            bloodTest.IsTestPassed = !( testDto.HivTest || testDto.HepatitisB || testDto.HepatitisC || testDto.Syphilis || testDto.Malaria );
            bloodTest.DonorId = testDto.DonorId;
            bloodTest.HospitalId = testDto.HospitalId;
            bloodTest.HospitalApprovalStatus = HospitalApprovalStatus.Pending;

            var result = await _bloodTestRepository.AddAsync( bloodTest );
            // Optionally update pending donations for this donor.
            var donorDonations = await _donationRepository.GetDonationsByDonorIdAsync( testDto.DonorId );
            foreach ( var donation in donorDonations.Where( d => d.Status == DonationStatus.Pending ) )
            {
                donation.Status = bloodTest.IsTestPassed ? DonationStatus.Approved : DonationStatus.Rejected;
                await _donationRepository.UpdateAsync( donation );
            }
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

            var donorDonations = await _donationRepository.GetDonationsByDonorIdAsync( bloodTest.DonorId );
            foreach ( var donation in donorDonations.Where( d => d.Status == DonationStatus.Pending ) )
            {
                donation.Status = bloodTest.IsTestPassed ? DonationStatus.Approved : DonationStatus.Rejected;
                await _donationRepository.UpdateAsync( donation );
            }
            return _mapper.Map<BloodTestDto>( bloodTest );
        }

        public async Task<BloodTestDto> GetTestByDonorIdAsync ( string donorId )
        {
            var test = await _bloodTestRepository.GetTestByDonorIdAsync( donorId );
            if ( test == null )
                throw new NotFoundException( $"Blood test for donor ID {donorId} not found" );
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

            bool isValid = !( test.HivTest || test.HepatitisB || test.HepatitisC || test.Syphilis || test.Malaria );
            if ( test.IsTestPassed != isValid )
            {
                test.IsTestPassed = isValid;
                await _bloodTestRepository.UpdateAsync( test );
                var donorDonations = await _donationRepository.GetDonationsByDonorIdAsync( test.DonorId );
                foreach ( var donation in donorDonations.Where( d => d.Status == DonationStatus.Pending ) )
                {
                    donation.Status = isValid ? DonationStatus.Approved : DonationStatus.Rejected;
                    await _donationRepository.UpdateAsync( donation );
                }
            }
            return isValid;
        }

        public async Task ApproveBloodTestAsync ( int id )
        {
            var test = await _bloodTestRepository.GetByIdAsync( id );
            if ( test == null )
                throw new NotFoundException( $"Blood test with ID {id} not found" );
            test.IsTestPassed = true;
            test.HospitalApprovalStatus = HospitalApprovalStatus.Approved;
            await _bloodTestRepository.UpdateAsync( test );

            var donorDonations = await _donationRepository.GetDonationsByDonorIdAsync( test.DonorId );
            foreach ( var donation in donorDonations.Where( d => d.Status == DonationStatus.Pending ) )
            {
                donation.Status = DonationStatus.Approved;
                await _donationRepository.UpdateAsync( donation );
            }
        }

        public async Task RejectBloodTestAsync ( int id )
        {
            var test = await _bloodTestRepository.GetByIdAsync( id );
            if ( test == null )
                throw new NotFoundException( $"Blood test with ID {id} not found" );
            test.HospitalApprovalStatus = HospitalApprovalStatus.Rejected;
            await _bloodTestRepository.UpdateAsync( test );

            var donorDonations = await _donationRepository.GetDonationsByDonorIdAsync( test.DonorId );
            foreach ( var donation in donorDonations.Where( d => d.Status == DonationStatus.Pending ) )
            {
                donation.Status = DonationStatus.Rejected;
                await _donationRepository.UpdateAsync( donation );
            }
        }
    }
}
