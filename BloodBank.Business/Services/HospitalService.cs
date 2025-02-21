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
    public class HospitalService : IHospitalService
    {
        private readonly IHospitalRepository _hospitalRepository;
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IMapper _mapper;

        public HospitalService (
            IHospitalRepository hospitalRepository,
            IBloodRequestRepository bloodRequestRepository,
            IMapper mapper )
        {
            _hospitalRepository = hospitalRepository;
            _bloodRequestRepository = bloodRequestRepository;
            _mapper = mapper;
        }

        public async Task<HospitalDto> GetHospitalByIdAsync ( int id )
        {
            var hospital = await _hospitalRepository.GetByIdAsync( id );
            if ( hospital == null )
                throw new NotFoundException( $"Hospital with ID {id} not found" );

            return _mapper.Map<HospitalDto>( hospital );
        }

        public async Task<IEnumerable<HospitalDto>> GetAllHospitalsAsync ()
        {
            var hospitals = await _hospitalRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<HospitalDto>>( hospitals );
        }

        public async Task<HospitalDto> CreateHospitalAsync ( CreateHospitalDto hospitalDto )
        {
            // Validate unique email and contact number
            if ( await IsEmailInUseAsync( hospitalDto.Email ) )
                throw new NotFoundException( "Email is already registered" );

            if ( await IsContactNumberInUseAsync( hospitalDto.ContactNumber ) )
                throw new NotFoundException( "Contact number is already registered" );

            // Validate email format
            if ( !IsValidEmail( hospitalDto.Email ) )
                throw new NotFoundException( "Invalid email format" );

            // Validate contact number format
            if ( !IsValidContactNumber( hospitalDto.ContactNumber ) )
                throw new NotFoundException( "Invalid contact number format" );

            var hospital = _mapper.Map<Hospital>( hospitalDto );
            var result = await _hospitalRepository.AddAsync( hospital );
            return _mapper.Map<HospitalDto>( result );
        }

        public async Task UpdateHospitalAsync ( int id, UpdateHospitalDto hospitalDto )
        {
            var hospital = await _hospitalRepository.GetByIdAsync( id );
            if ( hospital == null )
                throw new NotFoundException( $"Hospital with ID {id} not found" );

            // Check if new email is unique (if changed)
            if ( hospital.Email != hospitalDto.Email && await IsEmailInUseAsync( hospitalDto.Email ) )
                throw new NotFoundException( "Email is already registered" );

            // Check if new contact number is unique (if changed)
            if ( hospital.ContactNumber != hospitalDto.ContactNumber &&
                await IsContactNumberInUseAsync( hospitalDto.ContactNumber ) )
                throw new NotFoundException( "Contact number is already registered" );

            // Validate new email format
            if ( !IsValidEmail( hospitalDto.Email ) )
                throw new NotFoundException( "Invalid email format" );

            // Validate new contact number format
            if ( !IsValidContactNumber( hospitalDto.ContactNumber ) )
                throw new NotFoundException( "Invalid contact number format" );

            _mapper.Map( hospitalDto, hospital );
            await _hospitalRepository.UpdateAsync( hospital );
        }

        public async Task DeleteHospitalAsync ( int id )
        {
            var hospital = await _hospitalRepository.GetByIdAsync( id );
            if ( hospital == null )
                throw new NotFoundException( $"Hospital with ID {id} not found" );

            // Check if hospital has active blood requests
            var activeRequests = await _bloodRequestRepository.GetHospitalRequestsAsync( id );
            if ( activeRequests.Any( r => r.Status == RequestStatus.Pending ||
                                       r.Status == RequestStatus.Approved ) )
            {
                throw new NotFoundException( "Cannot delete hospital with active blood requests" );
            }

            await _hospitalRepository.DeleteAsync( hospital );
        }

        public async Task<IEnumerable<BloodRequestDto>> GetHospitalRequestsAsync ( int hospitalId )
        {
            if ( !await HospitalExistsAsync( hospitalId ) )
                throw new NotFoundException( $"Hospital with ID {hospitalId} not found" );

            var requests = await _bloodRequestRepository.GetHospitalRequestsAsync( hospitalId );
            return _mapper.Map<IEnumerable<BloodRequestDto>>( requests );
        }

        public async Task<HospitalDto> GetHospitalWithRequestsAsync ( int hospitalId )
        {
            var hospital = await _hospitalRepository.GetHospitalWithRequestsAsync( hospitalId );
            if ( hospital == null )
                throw new NotFoundException( $"Hospital with ID {hospitalId} not found" );

            return _mapper.Map<HospitalDto>( hospital );
        }

        public async Task<bool> ValidateHospitalCredentialsAsync ( string email, string contactNumber )
        {
            var hospital = await _hospitalRepository.GetByEmailAsync( email );
            if ( hospital == null )
                return false;

            return hospital.ContactNumber == contactNumber;
        }

        private async Task<bool> IsEmailInUseAsync ( string email )
        {
            var hospital = await _hospitalRepository.GetByEmailAsync( email );
            return hospital != null;
        }

        private async Task<bool> IsContactNumberInUseAsync ( string contactNumber )
        {
            var hospital = await _hospitalRepository.GetByContactNumberAsync( contactNumber );
            return hospital != null;
        }

        private async Task<bool> HospitalExistsAsync ( int id )
        {
            var hospital = await _hospitalRepository.GetByIdAsync( id );
            return hospital != null;
        }

        private bool IsValidEmail ( string email )
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress( email );
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidContactNumber ( string contactNumber )
        {
            // Example validation: must be 10-15 digits, can include '+' at start
            return System.Text.RegularExpressions.Regex.IsMatch(
                contactNumber,
                @"^\+?\d{10,15}$"
            );
        }
    }
}
