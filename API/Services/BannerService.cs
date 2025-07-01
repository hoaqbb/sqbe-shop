using API.Data.Entities;
using API.DTOs.BannerDtos;
using API.Interfaces;
using AutoMapper;

namespace API.Services
{
    public class BannerService : IBannerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BannerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<BannerDto>> GetBannersAsync()
        {
            var banners = await _unitOfWork.Repository<Banner>()
                .GetAllProjectedAsync<BannerDto>(b => b.IsActive, _mapper.ConfigurationProvider);

            return banners;
        }
    }
}
