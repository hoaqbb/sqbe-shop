namespace API.DTOs.SizeDtos
{
    public class SizeDetailDto : SizeDto
    {
        public DateTime CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int ProductCount { get; set; }
    }
}
