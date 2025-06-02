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

        public async Task<SizeDetailDto?> CreateAsync(string sizeName)
        {
            var newSize = new Size
            {
                Name = sizeName
            };

            await _unitOfWork.Repository<Size>().AddAsync(newSize);
            if(await _unitOfWork.SaveChangesAsync())
                return _mapper.Map<SizeDetailDto>(newSize);

            return null;
        }

        public async Task<SizeDetailDto?> UpdateAsync(int id, string value)
        {
            var size = await _unitOfWork.Repository<Size>().FindByIdAsync(id);
            if (size == null) return null;

            size.Name = value;
            size.UpdateAt = DateTime.UtcNow;

            _unitOfWork.Repository<Size>().Update(size);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<SizeDetailDto>(size);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var size = await _unitOfWork.Repository<Size>().FindByIdAsync(id);
            if (size == null) return false;

            _unitOfWork.Repository<Size>().Delete(size);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
