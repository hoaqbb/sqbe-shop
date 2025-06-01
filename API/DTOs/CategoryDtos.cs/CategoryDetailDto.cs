namespace API.DTOs.CategoryDtos.cs
{
    public class CategoryDetailDto : CategoryDto
    {
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int ProductCount { get; set; }
    }
}
