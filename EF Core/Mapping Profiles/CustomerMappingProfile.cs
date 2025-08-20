using AutoMapper;
using EF_Core.DTOs;
using EF_Core.Models;

namespace EF_Core.Mapping_Profiles
{
    public class CustomerMappingProfile : Profile
    {
        public CustomerMappingProfile()
        {
            CreateMap<CustomerDTO, Customer>()
                .ForMember(dest => dest.Cnic, opt => opt.MapFrom(src => src.Cnic))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

            CreateMap<Customer, CustomerDTO>()
                .ForMember(dest => dest.Cnic, opt => opt.MapFrom(src => src.Cnic))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
        }
    }
}
