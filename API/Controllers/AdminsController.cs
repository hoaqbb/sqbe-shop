using API.Data.Entities;
using API.DTOs.CategoryDtos.cs;
using API.DTOs.ColorDtos;
using API.DTOs.OrderDtos;
using API.DTOs.ProductDtos;
using API.DTOs.PromotionDtos;
using API.DTOs.SizeDtos;
using API.DTOs.UserDtos;
using API.Helpers.Params;
using API.Interfaces;
using API.Specifications.AdminSpecifications;
using API.Specifications.ProductSpecifications;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISizeService _sizeService;
        private readonly IColorService _colorService;
        private readonly IMapper _mapper;
        private readonly EcommerceDbContext _context;

        public AdminsController(IUnitOfWork unitOfWork, IProductService productService, ICategoryService categoryService, ISizeService sizeService, IColorService colorService, IMapper mapper, EcommerceDbContext context)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
            _categoryService = categoryService;
            _sizeService = sizeService;
            _colorService = colorService;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("products")]
        public async Task<ActionResult> GetProducts([FromQuery]AdminProductFilterParams param)
        {
            var spec = new AdminProductFilterSpecification(param);

            var result = await CreatePaginatedResult<Product, ProductListDto>(_unitOfWork.ProductRepository, spec, param.PageIndex, param.PageSize, _mapper.ConfigurationProvider);

            return Ok(result);
        }

        [HttpGet("promotions")]
        public async Task<ActionResult> GetPromotions()
        {
            var result = await _context.Promotions
                .ProjectTo<PromotionDetailDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("categories")]
        public async Task<ActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllWithDetailsAsync();

            return Ok(categories);
        }

        [HttpGet("colors")]
        public async Task<ActionResult> GetColors()
        {
            var colors = await _colorService.GetAllWithDetailsAsync();

            return Ok(colors);
        }

        [HttpGet("sizes")]
        public async Task<ActionResult> GetSizes()
        {
            var sizes = await _sizeService.GetAllWithDetailsAsync();

            return Ok(sizes);
        }
    }
}
