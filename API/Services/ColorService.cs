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

        public async Task<ColorDetailDto?> CreateAsync(ColorDto dto)
        {
            var newColor = new Color
            {
                Name = dto.Name,
                ColorCode = dto.ColorCode
            };

            await _unitOfWork.Repository<Color>().AddAsync(newColor);
            if (await _unitOfWork.SaveChangesAsync())
                return _mapper.Map<ColorDetailDto>(newColor);

            return null;
        }

        public async Task<ColorDetailDto?> UpdateAsync(int id, ColorDto dto)
        {
            var color = await _unitOfWork.Repository<Color>().FindByIdAsync(id);
            if (color == null) return null;

            color.Name = dto.Name;
            color.ColorCode = dto.ColorCode;
            color.UpdateAt = DateTime.UtcNow;

            _unitOfWork.Repository<Color>().Update(color);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ColorDetailDto>(color);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var color = await _unitOfWork.Repository<Color>().FindByIdAsync(id);
            if (color == null) return false;

            _unitOfWork.Repository<Color>().Delete(color);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
