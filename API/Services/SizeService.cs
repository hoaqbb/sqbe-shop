using API.Data.Entities;
using API.DTOs.SizeDtos;
using API.Interfaces;
using AutoMapper;

namespace API.Services
{
    public class SizeService : ISizeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SizeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<SizeDto>> GetAllAsync()
        {
            var sizes = await _unitOfWork.Repository<Size>()
                .GetAllProjectedAsync<SizeDto>(_mapper.ConfigurationProvider);

            return sizes;
        }

        public async Task<IReadOnlyList<SizeDetailDto>> GetAllWithDetailsAsync()
        {
            var sizes = await _unitOfWork.Repository<Size>()
                .GetAllProjectedAsync<SizeDetailDto>(_mapper.ConfigurationProvider);

            return sizes;
        }
    }
}
