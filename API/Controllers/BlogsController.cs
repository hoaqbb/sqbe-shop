using API.DTOs.BlogDtos;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;

        public BlogsController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        [HttpGet]
        public async Task<ActionResult> GetBlogs()
        {
            var blogs = await _blogService.GetBlogsAsync();

            return Ok(blogs);
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult> GetBlogBySlug(string slug)
        {
            var blog = await _blogService.GetBlogBySlugAsync(slug);
            if (blog == null)
            {
                return NotFound();
            }

            return Ok(blog);
        }

        [HttpPost]
        public async Task<ActionResult> CreateBlog([FromForm] CreateBlogDto createBlogDto)
        {
            var newBlog = await _blogService.CreateBlogAsync(createBlogDto);
            if(newBlog == null)
                return BadRequest("Failed to create blog.");

            return Ok(newBlog);
        }
    }
}
