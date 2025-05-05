using API.Data.Entities;
using API.DTOs.CartDtos;
using AutoMapper;

namespace API.Helpers.MappingProfiles
{
    public class CartProfiles : Profile
    {
        public CartProfiles()
        {
            CreateMap<Cart, CartDto>();

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                    src.ProductVariant.Product.Name))
                .ForMember(dest => dest.Photo, opt => opt.MapFrom(src =>
                    src.ProductVariant.Product.ProductImages.FirstOrDefault(x => x.IsMain).ImageUrl))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src =>
                    src.ProductVariant.Product.Price))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src =>
                    src.ProductVariant.Product.Discount))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src =>
                    src.ProductVariant.Product.Slug))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src =>
                    src.ProductVariant.Product.Category.Name))
                .ForMember(dest => dest.ProductVariant, opt => opt.MapFrom(src =>
                    src.ProductVariant));
        }
    }
}
