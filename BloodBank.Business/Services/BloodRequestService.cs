using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using BloodBank.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BloodBank.Application.Services
{
    public class BloodRequestService : IBloodRequestService
    {
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IBloodUnitRepository _bloodUnitRepository;

        public BloodRequestService (
            IBloodRequestRepository bloodRequestRepository,
            IBloodUnitRepository bloodUnitRepository )
        {
            _bloodRequestRepository = bloodRequestRepository;
            _bloodUnitRepository = bloodUnitRepository;
        }

        public async Task CreateBloodRequestAsync ( BloodRequest bloodRequest, string hospitalId )
        {
            bloodRequest.HospitalId = hospitalId;
            bloodRequest.RequestDate = DateTime.UtcNow;
            bloodRequest.CreatedAt = DateTime.UtcNow;
            bloodRequest.Status = RequestStatus.Pending; // Initial status
            bloodRequest.IsDeleted = false;
            bloodRequest.AssignedUnits = new List<BloodUnit>();

            await _bloodRequestRepository.AddAsync( bloodRequest );
        }

        public async Task<IEnumerable<BloodRequest>> GetHospitalRequestsAsync ( string hospitalId )
        {
            return await _bloodRequestRepository.GetAllByHospitalIdAsync( hospitalId );
        }

        public async Task<IEnumerable<BloodRequest>> GetPendingRequestsAsync ()
        {
            return await _bloodRequestRepository.GetPendingRequestsAsync();
        }

        // New method: Approve a request (Staff/Admin action)
        public async Task ApproveBloodRequestAsync ( int requestId, string staffId )
        {
            var request = await _bloodRequestRepository.GetByIdAsync( requestId );
            if ( request == null || request.Status != RequestStatus.Pending )
            {
                throw new InvalidOperationException( "Request not found or not in Pending status." );
            }

            request.Status = RequestStatus.Approved;
            request.UpdatedAt = DateTime.UtcNow;
            await _bloodRequestRepository.UpdateAsync( request );
        }

        // Updated method: Process an approved request (assign units)
        public async Task ProcessBloodRequestAsync ( int requestId, string staffId )
        {
            var request = await _bloodRequestRepository.GetByIdAsync( requestId );
            if ( request == null || request.Status != RequestStatus.Approved )
            {
                throw new InvalidOperationException( "Request not found or not approved for processing." );
            }

            request.Status = RequestStatus.InProcess; // Move to InProcess while assigning units
            var availableUnits = await _bloodUnitRepository.GetAvailableUnitsByBloodTypeAsync( request.BloodType );
            var requiredQuantity = request.QuantityRequired;
            var reservedUnits = new List<BloodUnit>();

            foreach ( var unit in availableUnits )
            {
                if ( requiredQuantity <= 0 ) break;

                reservedUnits.Add( unit );
                unit.Status = BloodUnitStatus.Reserved; // Reserve the unit
                requiredQuantity -= unit.Quantity;
            }

            request.AssignedUnits = reservedUnits;

            if ( requiredQuantity > 0 )
            {
                // Not enough units; stays InProcess
                request.Status = RequestStatus.InProcess;
            }
            else
            {
                // Fully fulfilled; move to Completed and dispatch units
                request.Status = RequestStatus.Completed;
                foreach ( var unit in reservedUnits )
                {
                    unit.Status = BloodUnitStatus.Dispatched;
                }
            }

            request.UpdatedAt = DateTime.UtcNow;
            await _bloodRequestRepository.UpdateAsync( request );
        }

        // New method: Reject a request (Staff/Admin action)
        public async Task RejectBloodRequestAsync ( int requestId, string staffId, string rejectionReason )
        {
            var request = await _bloodRequestRepository.GetByIdAsync( requestId );
            if ( request == null || ( request.Status != RequestStatus.Pending && request.Status != RequestStatus.Approved ) )
            {
                throw new InvalidOperationException( "Request not found or cannot be rejected." );
            }

            request.Status = RequestStatus.Rejected;
            request.Notes = $"{request.Notes}\nRejection Reason: {rejectionReason}".Trim();
            request.UpdatedAt = DateTime.UtcNow;
            await _bloodRequestRepository.UpdateAsync( request );
        }
    }
}