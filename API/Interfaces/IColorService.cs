using API.DTOs.ColorDtos;

namespace API.Interfaces
{
    public interface IColorService
    {
        Task<IReadOnlyList<ColorDto>> GetAllAsync();
        Task<IReadOnlyList<ColorDetailDto>> GetAllWithDetailsAsync();
    }
}
