using API.DTOs.ColorDtos;

namespace API.Interfaces
{
    public interface IColorService
    {
        Task<IReadOnlyList<ColorDto>> GetAllAsync();
        Task<IReadOnlyList<ColorDetailDto>> GetAllWithDetailsAsync();
        Task<ColorDetailDto?> CreateAsync(ColorDto dto);
        Task<ColorDetailDto?> UpdateAsync(int id, ColorDto dto);
        Task<bool> DeleteAsync(int id);
    }
}
