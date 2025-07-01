using API.DTOs.BannerDtos;

namespace API.Interfaces
{
    public interface IBannerService
    {
        Task<IReadOnlyList<BannerDto>> GetBannersAsync();
    }
}
