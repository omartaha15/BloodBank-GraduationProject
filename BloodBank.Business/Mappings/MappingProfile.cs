using AutoMapper;
using BloodBank.Business.DTOs;
using BloodBank.Core.Entities;
using BloodBank.Core.Entities.BloodBank.Core.Entities;
using BloodBank.Core.Enums;

namespace BloodBank.Business.Mappings
{
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
            CreateMap<User, UserDto>();

            // Donation Mappings
            CreateMap<Donation, DonationDto>()
                .ForMember( dest => dest.DonorName, opt => opt.MapFrom( src => $"{src.Donor.FirstName} {src.Donor.LastName}" ) )
                .ForMember( dest => dest.HospitalName, opt => opt.MapFrom( src => src.Hospital != null ? $"{src.Hospital.FirstName} {src.Hospital.LastName}" : "" ) )
                ;
            CreateMap<CreateDonationDto, Donation>();
            CreateMap<UpdateDonationDto, Donation>();

            // Blood Test Mappings
            CreateMap<BloodTest, BloodTestDto>()
                .ForMember( dest => dest.DonorName, opt => opt.MapFrom( src => src.Donor.FirstName + " " + src.Donor.LastName ) );
            CreateMap<CreateBloodTestDto, BloodTest>();
            CreateMap<UpdateBloodTestDto, BloodTest>();

            // Blood Unit Mappings (Assuming you have these DTOs)
            CreateMap<BloodUnit, BloodUnitDto>();
            CreateMap<CreateBloodUnitDto, BloodUnit>();

            // Blood Request Mappings
            CreateMap<BloodRequest, BloodRequestDto>()
                .ForMember( dest => dest.HospitalName, opt => opt.MapFrom( src => src.Hospital != null ? $"{src.Hospital.FirstName} {src.Hospital.LastName}" : "" ) );
            CreateMap<CreateBloodRequestDto, BloodRequest>();
            CreateMap<UpdateBloodRequestDto, BloodRequest>();

            // User Management Mappings
            CreateMap<User, UserManagementDto>()
                .ForMember( dest => dest.FullName, opt => opt.MapFrom( src => $"{src.FirstName} {src.LastName}" ) )
                .ForMember( dest => dest.IsActive, opt => opt.MapFrom( src => true ) )
                .ForMember( dest => dest.Roles, opt => opt.Ignore() );
        }
    }
}
