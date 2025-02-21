using AutoMapper;
using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using OpenQA.Selenium;

namespace BloodBank.Business.Services
{
    public class BloodUnitService : IBloodUnitService
    {
        private readonly IBloodUnitRepository _bloodUnitRepository;
        private readonly IDonationRepository _donationRepository;
        private readonly IMapper _mapper;
        private const int BLOOD_EXPIRY_DAYS = 42; // Blood expires after 42 days

        public BloodUnitService (
            IBloodUnitRepository bloodUnitRepository,
            IDonationRepository donationRepository,
            IMapper mapper )
        {
            _bloodUnitRepository = bloodUnitRepository;
            _donationRepository = donationRepository;
            _mapper = mapper;
        }

        public async Task<BloodUnitDto> GetBloodUnitByIdAsync ( int id )
        {
            var unit = await _bloodUnitRepository.GetByIdAsync( id );
            if ( unit == null )
                throw new NotFoundException( $"Blood unit with ID {id} not found" );

            return _mapper.Map<BloodUnitDto>( unit );
        }

        public async Task<BloodUnitDto> CreateBloodUnitAsync ( CreateBloodUnitDto unitDto )
        {
            // Validate donation exists and is approved
            var donation = await _donationRepository.GetByIdAsync( unitDto.DonationId );
            if ( donation == null )
                throw new NotFoundException( $"Donation with ID {unitDto.DonationId} not found" );

            if ( donation.Status != DonationStatus.Approved )
                throw new NotFoundException( "Cannot create blood unit for non-approved donation" );

            var bloodUnit = _mapper.Map<BloodUnit>( unitDto );
            bloodUnit.UnitNumber = GenerateUnitNumber();
            bloodUnit.Status = BloodUnitStatus.Available;
            bloodUnit.CollectionDate = DateTime.UtcNow;
            bloodUnit.ExpiryDate = DateTime.UtcNow.AddDays( BLOOD_EXPIRY_DAYS );

            // Validate blood type matches donation
            bloodUnit.BloodType = donation.BloodType;

            var result = await _bloodUnitRepository.AddAsync( bloodUnit );
            return _mapper.Map<BloodUnitDto>( result );
        }

        public async Task UpdateUnitStatusAsync ( int id, BloodUnitStatus status )
        {
            var unit = await _bloodUnitRepository.GetByIdAsync( id );
            if ( unit == null )
                throw new NotFoundException( $"Blood unit with ID {id} not found" );

            // Validate status transition
            if ( !IsValidStatusTransition( unit.Status, status ) )
                throw new NotFoundException( $"Invalid status transition from {unit.Status} to {status}" );

            unit.Status = status;
            await _bloodUnitRepository.UpdateAsync( unit );
        }

        public async Task<IEnumerable<BloodUnitDto>> GetAvailableUnitsByBloodTypeAsync ( BloodType bloodType )
        {
            var units = await _bloodUnitRepository.GetAvailableUnitsByBloodTypeAsync( bloodType );
            return _mapper.Map<IEnumerable<BloodUnitDto>>( units );
        }

        public async Task<IEnumerable<BloodUnitDto>> GetExpiringUnitsAsync ( int daysThreshold )
        {
            var units = await _bloodUnitRepository.GetExpiringUnitsAsync( daysThreshold );
            return _mapper.Map<IEnumerable<BloodUnitDto>>( units );
        }

        public async Task<BloodUnitDto> GetUnitByBarcodeAsync ( string barcode )
        {
            var unit = await _bloodUnitRepository.GetUnitByBarcodeAsync( barcode );
            if ( unit == null )
                throw new NotFoundException( $"Blood unit with barcode {barcode} not found" );

            return _mapper.Map<BloodUnitDto>( unit );
        }

        public async Task<int> GetAvailableUnitsCountByBloodTypeAsync ( BloodType bloodType )
        {
            return await _bloodUnitRepository.GetAvailableUnitsCountByBloodTypeAsync( bloodType );
        }

        public async Task<IEnumerable<BloodUnitDto>> GetUnitsByDonationIdAsync ( int donationId )
        {
            var units = await _bloodUnitRepository.GetUnitsByDonationIdAsync( donationId );
            return _mapper.Map<IEnumerable<BloodUnitDto>>( units );
        }

        private string GenerateUnitNumber ()
        {
            // Generate unique barcode following ISBT 128 standard
            // Format: BU[YearMonthDay][8-character-unique-id]
            return $"BU{DateTime.UtcNow:yyyyMMdd}{Guid.NewGuid().ToString().Substring( 0, 8 )}";
        }

        private bool IsValidStatusTransition ( BloodUnitStatus currentStatus, BloodUnitStatus newStatus )
        {
            // Define valid status transitions
            switch ( currentStatus )
            {
                case BloodUnitStatus.Available:
                    return newStatus == BloodUnitStatus.Reserved ||
                           newStatus == BloodUnitStatus.Discarded ||
                           newStatus == BloodUnitStatus.Expired;

                case BloodUnitStatus.Reserved:
                    return newStatus == BloodUnitStatus.Available ||
                           newStatus == BloodUnitStatus.Dispatched;

                case BloodUnitStatus.Dispatched:
                    return false; // Final state

                case BloodUnitStatus.Expired:
                    return false; // Final state

                case BloodUnitStatus.Discarded:
                    return false; // Final state

                default:
                    return false;
            }
        }
    }
}
