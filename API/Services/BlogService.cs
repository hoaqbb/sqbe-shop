using API.Data.Entities;
using API.DTOs.BlogDtos;
using API.Interfaces;
using AutoMapper;

namespace API.Services
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BlogService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BlogDto>> GetBlogsAsync()
        {
            var blogs = await _unitOfWork.Repository<Blog>()
                .GetAllProjectedAsync<BlogDto>(
                    b => b.Status == true,
                    _mapper.ConfigurationProvider
                );

            return blogs.OrderByDescending(b => b.CreateAt);
        }

        public async Task<BlogDetailDto?> GetBlogBySlugAsync(string slug)
        {
            var blog = await _unitOfWork.Repository<Blog>()
                .GetSingleProjectedAsync<BlogDetailDto>(
                    b => b.Slug == slug && b.Status == true,
                    _mapper.ConfigurationProvider
                );

            return blog;
        }
    }
}
