namespace API.DTOs.ColorDtos
{
    public class ColorDetailDto : ColorDto
    {
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int ProductCount { get; set; }
    }
}
