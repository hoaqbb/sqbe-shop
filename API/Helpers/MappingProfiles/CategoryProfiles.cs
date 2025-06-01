using API.Data.Entities;
using API.DTOs.CategoryDtos.cs;
using AutoMapper;

namespace API.Helpers.MappingProfiles
{
    public class CategoryProfiles : Profile
    {
        public CategoryProfiles()
        {
            CreateMap<Category, CategoryDto>();

            CreateMap<Category, CategoryDetailDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src =>
                    src.Products.Count));
        }
    }
}
