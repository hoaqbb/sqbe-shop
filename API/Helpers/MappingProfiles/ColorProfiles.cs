using API.Data.Entities;
using API.DTOs.ColorDtos;
using AutoMapper;

namespace API.Helpers.MappingProfiles
{
    public class ColorProfiles : Profile
    {
        public ColorProfiles()
        {
            CreateMap<Color, ColorDto>();

            CreateMap<Color, ColorDetailDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src =>
                    src.ProductColors.Count()));
        }
    }
}
