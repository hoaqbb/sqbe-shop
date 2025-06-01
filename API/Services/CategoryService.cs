using API.DTOs.CategoryDtos.cs;
using API.Interfaces;
using AutoMapper;

namespace API.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<CategoryDto>> GetAllAsync()
        {
            var categories = await _unitOfWork.CategoryRepository
                .GetAllProjectedAsync<CategoryDto>(_mapper.ConfigurationProvider);

            return categories;
        }

        public async Task<IReadOnlyList<CategoryDetailDto>> GetAllWithDetailsAsync()
        {
            var categories = await _unitOfWork.CategoryRepository
                .GetAllProjectedAsync<CategoryDetailDto>(_mapper.ConfigurationProvider);

            return categories;
        }
    }
}
