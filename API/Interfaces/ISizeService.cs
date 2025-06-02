using API.DTOs.CategoryDtos.cs;
using API.DTOs.SizeDtos;

namespace API.Interfaces
{
    public interface ISizeService
    {
        Task<IReadOnlyList<SizeDto>> GetAllAsync();
        Task<IReadOnlyList<SizeDetailDto>> GetAllWithDetailsAsync();
        Task<SizeDetailDto?> CreateAsync(string sizeName);
        Task<SizeDetailDto?> UpdateAsync(int id, string value);
        Task<bool> DeleteAsync(int id);
    }
}
