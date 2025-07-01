using API.Data.Entities;
using API.Data.Enums;
using API.DTOs.BannerDtos;
using AutoMapper;

namespace API.Helpers.MappingProfiles
{
    public class BannerProfiles : Profile
    {
        public BannerProfiles()
        {
            CreateMap<Banner, BannerDto>()
                .ForMember(dest => dest.DisplayType, opt => opt.MapFrom(src => (BannerDisplayTypeEnum)src.DisplayType));
        }
    }
}
