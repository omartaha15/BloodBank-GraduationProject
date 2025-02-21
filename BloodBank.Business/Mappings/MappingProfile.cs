using BloodBank.Business.DTOs;
using BloodBank.Core.Entities;
using OpenQA.Selenium.DevTools.V130.Profiler;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Profile = AutoMapper.Profile;

namespace BloodBank.Business.Mappings
{
    // BloodBank.Application/Mappings/MappingProfile.cs
    public class MappingProfile : Profile
    {
        public MappingProfile ()
        {
            // User Mappings
            CreateMap<RegisterDto, User>()
                .ForMember( dest => dest.UserName, opt => opt.MapFrom( src => src.Email ) )
                .ForMember( dest => dest.Email, opt => opt.MapFrom( src => src.Email ) )
                .ForMember( dest => dest.FirstName, opt => opt.MapFrom( src => src.FirstName ) )
                .ForMember( dest => dest.LastName, opt => opt.MapFrom( src => src.LastName ) )
                .ForMember( dest => dest.PhoneNumber, opt => opt.MapFrom( src => src.PhoneNumber ) )
                .ForMember( dest => dest.DateOfBirth, opt => opt.MapFrom( src => src.DateOfBirth ) )
                .ForMember( dest => dest.Address, opt => opt.MapFrom( src => src.Address ) )
                .ForMember( dest => dest.Gender, opt => opt.MapFrom( src => src.Gender ) )
                .ForMember( dest => dest.BloodType, opt => opt.MapFrom( src => src.BloodType ) );

            CreateMap<User, UserDto>()
                ;

            // Donation Mappings
            CreateMap<Donation, DonationDto>()
                .ForMember( dest => dest.DonorName,
                    opt => opt.MapFrom( src => $"{src.Donor.FirstName} {src.Donor.LastName}" ) );
            CreateMap<CreateDonationDto, Donation>();
            CreateMap<UpdateDonationDto, Donation>();

            // Blood Test Mappings
            CreateMap<BloodTest, BloodTestDto>();
            CreateMap<CreateBloodTestDto, BloodTest>();
            CreateMap<UpdateBloodTestDto, BloodTest>();

            // Blood Unit Mappings
            CreateMap<BloodUnit, BloodUnitDto>();
            CreateMap<CreateBloodUnitDto, BloodUnit>();
            //CreateMap<UpdateBloodUnitDto, BloodUnit>();

            // Appointment Mappings
            CreateMap<Appointment, AppointmentDto>()
                .ForMember( dest => dest.DonorName,
                    opt => opt.MapFrom( src => $"{src.Donor.FirstName} {src.Donor.LastName}" ) );
            CreateMap<CreateAppointmentDto, Appointment>();
            CreateMap<UpdateAppointmentDto, Appointment>();

            // Hospital Mappings
            CreateMap<Hospital, HospitalDto>();
            CreateMap<CreateHospitalDto, Hospital>();
            CreateMap<UpdateHospitalDto, Hospital>();

            // Blood Request Mappings
            CreateMap<BloodRequest, BloodRequestDto>()
                .ForMember( dest => dest.HospitalName,
                    opt => opt.MapFrom( src => src.Hospital.Name ) );
            CreateMap<CreateBloodRequestDto, BloodRequest>();
            CreateMap<UpdateBloodRequestDto, BloodRequest>();


            CreateMap<User, UserManagementDto>()
            .ForMember( dest => dest.FullName,
                opt => opt.MapFrom( src => $"{src.FirstName} {src.LastName}" ) )
            .ForMember( dest => dest.IsActive,
                opt => opt.MapFrom( src => true ) ) // Or your logic for IsActive
            .ForMember( dest => dest.Roles,
                opt => opt.Ignore() ); // We'll set roles manually
        }
    }
}
