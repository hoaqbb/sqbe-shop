using API.Data.Entities;
using API.Data.Enums;
using API.DTOs.PromotionDtos;
using AutoMapper;

namespace API.Helpers.MappingProfiles
{
    public class PromotionProfiles : Profile
    {
        public PromotionProfiles()
        {
            CreateMap<Promotion, PromotionDto>()
                .ForMember(dest => dest.DiscountType, opt => opt.MapFrom(src => (DiscountTypeEnum)src.DiscountType));

            CreateMap<Promotion, PromotionDetailDto>();
        }
    }
}
