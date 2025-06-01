using API.DTOs.CategoryDtos.cs;
using System.Collections.Generic;

namespace API.Interfaces
{
    public interface ICategoryService
    {
        Task<IReadOnlyList<CategoryDto>> GetAllAsync();
        Task<IReadOnlyList<CategoryDetailDto>> GetAllWithDetailsAsync();
    }
}
