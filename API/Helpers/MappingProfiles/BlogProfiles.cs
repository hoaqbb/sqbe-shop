using API.Data.Entities;
using API.DTOs.BlogDtos;
using AutoMapper;

namespace API.Helpers.MappingProfiles
{
    public class BlogProfiles : Profile
    {
        public BlogProfiles()
        {
            CreateMap<Blog, BlogDto>();
            CreateMap<Blog, BlogDetailDto>();
        }
    }
}
