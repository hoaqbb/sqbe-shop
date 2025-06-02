using System.ComponentModel.DataAnnotations;

namespace API.DTOs.CategoryDtos.cs
{
    public class UpdateCategoryDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Slug { get; set; }
    }
}
