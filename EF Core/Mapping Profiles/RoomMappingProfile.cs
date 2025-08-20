using AutoMapper;
using EF_Core.DTOs;
using EF_Core.Models;

namespace EF_Core.Mapping_Profiles
{
    public class RoomMappingProfile : Profile
    {
        public RoomMappingProfile()
        {
            CreateMap<RoomDTO, Room>()
                .ForMember(dest => dest.RoomNo, opt => opt.MapFrom(src => src.RoomNo))
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Capacity))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<Room, RoomDTO>()
                .ForMember(dest => dest.RoomNo, opt => opt.MapFrom(src => src.RoomNo))
                .ForMember(dest => dest.Capacity, opt => opt.MapFrom(src => src.Capacity))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
        }
    }
}
