using API.Data.Entities;
using API.DTOs.ColorDtos;
using API.Interfaces;
using AutoMapper;

namespace API.Services
{
    public class ColorService : IColorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ColorService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IReadOnlyList<ColorDto>> GetAllAsync()
        {
            var colors = await _unitOfWork.Repository<Color>()
                .GetAllProjectedAsync<ColorDto>(_mapper.ConfigurationProvider);

            return colors;
        }

        public async Task<IReadOnlyList<ColorDetailDto>> GetAllWithDetailsAsync()
        {
            var colors = await _unitOfWork.Repository<Color>()
                .GetAllProjectedAsync<ColorDetailDto>(_mapper.ConfigurationProvider);

            return colors;
        }
    }
}
