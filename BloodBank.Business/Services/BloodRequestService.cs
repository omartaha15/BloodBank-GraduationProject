using AutoMapper;
using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using OpenQA.Selenium;

namespace BloodBank.Business.Services
{
    // BloodBank.Application/Services/BloodRequestService.cs
    public class BloodRequestService : IBloodRequestService
    {
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IHospitalRepository _hospitalRepository;
        private readonly IBloodUnitRepository _bloodUnitRepository;
        private readonly IMapper _mapper;

        public BloodRequestService (
            IBloodRequestRepository bloodRequestRepository,
            IHospitalRepository hospitalRepository,
            IBloodUnitRepository bloodUnitRepository,
            IMapper mapper )
        {
            _bloodRequestRepository = bloodRequestRepository;
            _hospitalRepository = hospitalRepository;
            _bloodUnitRepository = bloodUnitRepository;
            _mapper = mapper;
        }

        public async Task<BloodRequestDto> GetRequestByIdAsync ( int id )
        {
            var request = await _bloodRequestRepository.GetByIdAsync( id );
            if ( request == null )
                throw new NotFoundException( $"Blood request with ID {id} not found" );

            return _mapper.Map<BloodRequestDto>( request );
        }

        public async Task<IEnumerable<BloodRequestDto>> GetAllRequestsAsync ()
        {
            var requests = await _bloodRequestRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BloodRequestDto>>( requests );
        }

        public async Task<BloodRequestDto> CreateRequestAsync ( CreateBloodRequestDto requestDto )
        {
            // Validate hospital exists
            var hospital = await _hospitalRepository.GetByIdAsync( requestDto.HospitalId );
            if ( hospital == null )
                throw new NotFoundException( $"Hospital with ID {requestDto.HospitalId} not found" );

            // Validate required date is not in the past
            if ( requestDto.RequiredDate < DateTime.UtcNow )
                throw new NotFoundException( "Required date cannot be in the past" );

            // For emergency requests, validate the priority
            if ( requestDto.Priority == RequestPriority.Emergency )
            {
                // Emergency requests must be needed within 24 hours
                if ( requestDto.RequiredDate > DateTime.UtcNow.AddHours( 24 ) )
                    throw new NotFoundException( "Emergency requests must be needed within 24 hours" );
            }

            var request = _mapper.Map<BloodRequest>( requestDto );
            request.Status = RequestStatus.Pending;

            var result = await _bloodRequestRepository.AddAsync( request );
            return _mapper.Map<BloodRequestDto>( result );
        }

        public async Task UpdateRequestStatusAsync ( int id, RequestStatus status )
        {
            var request = await _bloodRequestRepository.GetByIdAsync( id );
            if ( request == null )
                throw new NotFoundException( $"Blood request with ID {id} not found" );

            // Validate status transition
            if ( !IsValidStatusTransition( request.Status, status ) )
                throw new NotFoundException( $"Invalid status transition from {request.Status} to {status}" );

            request.Status = status;
            await _bloodRequestRepository.UpdateAsync( request );
        }

        public async Task<IEnumerable<BloodRequestDto>> GetPendingRequestsAsync ()
        {
            var requests = await _bloodRequestRepository.GetPendingRequestsAsync();
            return _mapper.Map<IEnumerable<BloodRequestDto>>( requests );
        }

        public async Task<IEnumerable<BloodRequestDto>> GetHospitalRequestsAsync ( int hospitalId )
        {
            // Validate hospital exists
            var hospital = await _hospitalRepository.GetByIdAsync( hospitalId );
            if ( hospital == null )
                throw new NotFoundException( $"Hospital with ID {hospitalId} not found" );

            var requests = await _bloodRequestRepository.GetHospitalRequestsAsync( hospitalId );
            return _mapper.Map<IEnumerable<BloodRequestDto>>( requests );
        }

        public async Task<IEnumerable<BloodRequestDto>> GetRequestsByBloodTypeAsync ( BloodType bloodType )
        {
            var requests = await _bloodRequestRepository.GetRequestsByBloodTypeAsync( bloodType );
            return _mapper.Map<IEnumerable<BloodRequestDto>>( requests );
        }

        public async Task<IEnumerable<BloodRequestDto>> GetUrgentRequestsAsync ()
        {
            var requests = await _bloodRequestRepository.GetUrgentRequestsAsync();
            return _mapper.Map<IEnumerable<BloodRequestDto>>( requests );
        }

        public async Task AssignBloodUnitsToRequestAsync ( int requestId, List<int> bloodUnitIds )
        {
            var request = await _bloodRequestRepository.GetByIdAsync( requestId );
            if ( request == null )
                throw new NotFoundException( $"Blood request with ID {requestId} not found" );

            // Validate request status
            if ( request.Status != RequestStatus.Pending )
                throw new NotFoundException( "Can only assign blood units to pending requests" );

            // Validate blood units exist and are available
            foreach ( var unitId in bloodUnitIds )
            {
                var unit = await _bloodUnitRepository.GetByIdAsync( unitId );
                if ( unit == null )
                    throw new NotFoundException( $"Blood unit with ID {unitId} not found" );

                if ( unit.Status != BloodUnitStatus.Available )
                    throw new NotFoundException( $"Blood unit {unitId} is not available" );

                if ( unit.BloodType != request.BloodType )
                    throw new NotFoundException( $"Blood unit {unitId} blood type does not match request blood type" );

                // Update blood unit status
                unit.Status = BloodUnitStatus.Reserved;
                await _bloodUnitRepository.UpdateAsync( unit );
            }

            // Calculate total quantity of assigned units
            var assignedUnits = await Task.WhenAll( bloodUnitIds.Select( id => _bloodUnitRepository.GetByIdAsync( id ) ) );
            var totalQuantity = assignedUnits.Sum( u => u.Quantity );

            // Validate quantity meets request
            if ( totalQuantity < request.QuantityRequired )
                throw new NotFoundException( "Assigned units do not meet required quantity" );

            // Update request status
            request.Status = RequestStatus.InProcess;
            await _bloodRequestRepository.UpdateAsync( request );
        }

        private bool IsValidStatusTransition ( RequestStatus currentStatus, RequestStatus newStatus )
        {
            switch ( currentStatus )
            {
                case RequestStatus.Pending:
                    return newStatus == RequestStatus.Approved ||
                           newStatus == RequestStatus.Rejected ||
                           newStatus == RequestStatus.InProcess;

                case RequestStatus.Approved:
                    return newStatus == RequestStatus.InProcess ||
                           newStatus == RequestStatus.Completed;

                case RequestStatus.InProcess:
                    return newStatus == RequestStatus.Completed;

                case RequestStatus.Completed:
                case RequestStatus.Rejected:
                    return false; // Final states

                default:
                    return false;
            }
        }
    }
}
