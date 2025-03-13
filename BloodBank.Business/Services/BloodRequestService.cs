using AutoMapper;
using BloodBank.Business.DTOs;
using BloodBank.Business.Interfaces;
using BloodBank.Core.Entities;
using BloodBank.Core.Enums;
using BloodBank.Core.Interfaces;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BloodBank.Business.Services
{
    public class BloodRequestService : IBloodRequestService
    {
        private readonly IBloodRequestRepository _bloodRequestRepository;
        private readonly IMapper _mapper;

        public BloodRequestService ( IBloodRequestRepository bloodRequestRepository, IMapper mapper )
        {
            _bloodRequestRepository = bloodRequestRepository;
            _mapper = mapper;
        }

        public async Task<BloodRequestDto> CreateBloodRequestAsync ( CreateBloodRequestDto dto )
        {
            var bloodRequest = _mapper.Map<BloodRequest>( dto );
            bloodRequest.RequestDate = DateTime.UtcNow;
            bloodRequest.Status = RequestStatus.Pending;
            var result = await _bloodRequestRepository.AddAsync( bloodRequest );
            return _mapper.Map<BloodRequestDto>( result );
        }

        public async Task<IEnumerable<BloodRequestDto>> GetAllBloodRequestsAsync ()
        {
            var requests = await _bloodRequestRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<BloodRequestDto>>( requests );
        }

        public async Task ApproveBloodRequestAsync ( int requestId )
        {
            var request = await _bloodRequestRepository.GetByIdAsync( requestId );
            if ( request == null )
                throw new NotFoundException( $"Blood request with ID {requestId} not found" );
            request.Status = RequestStatus.Approved;
            await _bloodRequestRepository.UpdateAsync( request );
        }

        public async Task RejectBloodRequestAsync ( int requestId )
        {
            var request = await _bloodRequestRepository.GetByIdAsync( requestId );
            if ( request == null )
                throw new NotFoundException( $"Blood request with ID {requestId} not found" );
            request.Status = RequestStatus.Rejected;
            await _bloodRequestRepository.UpdateAsync( request );
        }
    }
}
