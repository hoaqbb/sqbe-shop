using API.Data.Entities;
using API.DTOs.SizeDtos;
using AutoMapper;

namespace API.Helpers.MappingProfiles
{
    public class SizeProfiles : Profile
    {
        public SizeProfiles()
        {
            CreateMap<Size, SizeDto>();

            CreateMap<Size, SizeDetailDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src =>
                    src.ProductVariants.Select(x => x.ProductId).Distinct().Count()));
        }
    }
}
