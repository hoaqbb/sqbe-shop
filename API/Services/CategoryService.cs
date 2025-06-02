using API.Data.Entities;
using API.DTOs.CategoryDtos.cs;
using API.Interfaces;
using AutoMapper;
using StackExchange.Redis;

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

        public async Task<CategoryDetailDto?> CreateAsync(CreateCategoryDto dto)
        {
            var isSlugExisted = await IsSlugExistsAsync(dto.Slug);
            if (isSlugExisted) return null;

            var newCat = new Category
            {
                Name = dto.Name,
                Slug = dto.Slug
            };

            await _unitOfWork.Repository<Category>().AddAsync(newCat);
            if (await _unitOfWork.SaveChangesAsync())
                return _mapper.Map<CategoryDetailDto>(newCat);

            return null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _unitOfWork.CategoryRepository.FindByIdAsync(id);
            if (category == null)
            {
                return false;
            }

            _unitOfWork.CategoryRepository.Delete(category);

            if (await _unitOfWork.SaveChangesAsync())
            {
                return true;
            }
            
            return false;
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

        public async Task<CategoryDetailDto?> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _unitOfWork.CategoryRepository.FindByIdAsync(id);
            if (category == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Slug) && dto.Slug != category.Slug)
            {
                var isSlugExisted = await IsSlugExistsAsync(dto.Slug);
                if (isSlugExisted)
                {
                    return null;
                }
            }

            category.Name = dto.Name;
            category.Slug = dto.Slug;
            category.UpdateAt = DateTime.UtcNow;

            _unitOfWork.CategoryRepository.Update(category);

            var saveResult = await _unitOfWork.SaveChangesAsync();
            if (!saveResult)
            {
                return null;
            }

            var result = _mapper.Map<CategoryDetailDto>(category);

            return result;
        }

        private async Task<bool> IsSlugExistsAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug)) return false;

            return await _unitOfWork.CategoryRepository.ExistsAsync(c => c.Slug == slug);
        }
    }
}
