using API.DTOs.SizeDtos;

namespace API.Interfaces
{
    public interface ISizeService
    {
        Task<IReadOnlyList<SizeDto>> GetAllAsync();
        Task<IReadOnlyList<SizeDetailDto>> GetAllWithDetailsAsync();
    }
}
