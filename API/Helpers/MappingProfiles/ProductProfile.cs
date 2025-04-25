using API.Data.Entities;
using API.DTOs.ProductDtos;
using AutoMapper;

namespace API.Helpers.MappingProfiles
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductListDto>()
                .ForMember(dest => dest.MainPhoto, opt => opt.MapFrom(src =>
                    src.ProductImages.FirstOrDefault(x => x.IsMain).ImageUrl))
                .ForMember(dest => dest.SubPhoto, opt => opt.MapFrom(src =>
                    src.ProductImages.FirstOrDefault(x => x.IsSub).ImageUrl))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
                    src.Category.Name));

            CreateMap<Product, ProductDetailDto>();

            CreateMap<ProductImage, ProductImageDto>();

            CreateMap<ProductVariant, ProductVariantDto>()
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src =>
                    src.ProductColor.Color.Name))
                .ForMember(dest => dest.ColorCode, opt => opt.MapFrom(src =>
                    src.ProductColor.Color.ColorCode))
                .ForMember(dest => dest.Size, opt => opt.MapFrom(src =>
                    src.Size.Name));
        }
    }
}
