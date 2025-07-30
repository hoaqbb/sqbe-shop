using API.DTOs.BlogDtos;

namespace API.Interfaces
{
    public interface IBlogService
    {
        Task<IEnumerable<BlogDto>> GetBlogsAsync();
        Task<BlogDetailDto?> GetBlogBySlugAsync(string slug);
        Task<BlogDto?> CreateBlogAsync(CreateBlogDto createBlogDto);
    }
}
