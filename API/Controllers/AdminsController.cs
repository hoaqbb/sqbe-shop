using API.Data.Entities;
using API.DTOs.CategoryDtos.cs;
using API.DTOs.OrderDtos;
using API.DTOs.ProductDtos;
using API.Helpers.Params;
using API.Interfaces;
using API.Specifications.AdminSpecifications;
using API.Specifications.ProductSpecifications;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IMapper _mapper;
        private readonly EcommerceDbContext _context;

        public AdminsController(IUnitOfWork unitOfWork, IProductService productService, ICategoryService categoryService, IMapper mapper, EcommerceDbContext context)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
            _categoryService = categoryService;
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

        [HttpGet("categories")]
        public async Task<ActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllWithDetailsAsync();

            return Ok(categories);
        }

    }
}
