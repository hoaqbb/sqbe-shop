using API.Data.Entities;
using API.DTOs.BlogDtos;
using API.Interfaces;
using AutoMapper;
using Slugify;

namespace API.Services
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;

        public BlogService(IUnitOfWork unitOfWork, IMapper mapper, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _imageService = imageService;
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

        public async Task<BlogDto?> CreateBlogAsync(CreateBlogDto createBlogDto)
        {
            var randomNumber = new Random().Next(1000000);

            var thumb = await _imageService.AddImageAsync(createBlogDto.ThumbnailFile);
            var blog = new Blog
            {
                Title = createBlogDto.Title,
                Slug = new SlugHelper()
                .GenerateSlug(createBlogDto.Title + " " + randomNumber.ToString()),
                Excerpt = createBlogDto.Excerpt,
                Content = createBlogDto.Content,
                Status = createBlogDto.Status,
                ThumbnailUrl = thumb.SecureUrl.AbsoluteUri
            };

            await _unitOfWork.Repository<Blog>().AddAsync(blog);
            if(await _unitOfWork.SaveChangesAsync())
                return _mapper.Map<BlogDto>(blog);

            return null;
        }
    }
}
