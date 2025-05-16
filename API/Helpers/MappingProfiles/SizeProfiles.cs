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
        }
    }
}
