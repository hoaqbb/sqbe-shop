using API.Data.Enums;

namespace API.DTOs.BannerDtos
{
    public class BannerDto
    {
        public string ImageUrl { get; set; } = null!;
        public string? LinkUrl { get; set; }
        public string DisplayType { get; set; }
    }
}
