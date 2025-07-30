using System.ComponentModel.DataAnnotations;

namespace API.DTOs.BlogDtos
{
    public class CreateBlogDto
    {
        [Required]
        public string Title { get; set; } = null!;
        public string? Excerpt { get; set; }
        [Required]
        public string Content { get; set; } = null!;
        [Required]
        public bool Status { get; set; }
        [Required]
        public IFormFile ThumbnailFile { get; set; } = null!;
    }
}
