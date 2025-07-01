namespace API.DTOs.BlogDtos
{
    public class BlogDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string Content { get; set; } = null!;
        public bool Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Excerpt { get; set; }
        public string ThumbnailUrl { get; set; } = null!;
    }
}
