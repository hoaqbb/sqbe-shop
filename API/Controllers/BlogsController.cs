using API.Data.Entities;
using API.DTOs.BlogDtos;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly IBlogService _blogService;
        private readonly IMapper _mapper;

        public BlogsController(IBlogService blogService, IMapper mapper)
        {
            _blogService = blogService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetBlogs()
        {
            var blogs = await _blogService.GetBlogsAsync(true);

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
    }
}
