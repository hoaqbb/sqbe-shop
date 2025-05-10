using API.Data.Entities;
using API.DTOs.ProductDtos;
using API.Extensions;
using API.Helpers.Params;
using API.Interfaces;
using API.Specifications.ProductSpecifications;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork unitOfWork, ITokenService tokenService, IProductService productService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet()]
        public async Task<ActionResult> GetProducts([FromQuery]ProductFilterParams param)
        {
            var userId = HttpContext.GetUserIdFromTokenInsideCookie(_tokenService);
            var spec = new ProductFilterSpecification(param);

            var result = await CreatePaginatedResult<Product, ProductListDto>(_unitOfWork.ProductRepository, spec, param.PageIndex, param.PageSize, _mapper.ConfigurationProvider);

            if (userId != null)
                result = await _productService.MarkLikedProductsAsync(result, (Guid)userId);

            return Ok(result);
        }

        [HttpGet("{slug}", Name = "GetProduct")]
        public async Task<ActionResult> GetProductBySlug(string slug)
        {
            var product = await _unitOfWork.ProductRepository
                .GetSingleProjectedAsync<ProductDetailDto>(
                    (x => x.Slug == slug && x.IsVisible == true), 
                    _mapper.ConfigurationProvider
                );

            if (product == null) return NotFound("Product not found!");

            return Ok(product);
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchProduct([FromQuery]ProductSearchParams param)
        {
            if (string.IsNullOrWhiteSpace(param.Keyword))
            {
                return NoContent();
            }
            var userId = HttpContext.GetUserIdFromTokenInsideCookie(_tokenService);
            var spec = new ProductSearchSpecification(param);

            var result = await CreatePaginatedResult<Product, ProductListDto>(_unitOfWork.ProductRepository, spec, param.PageIndex, param.PageSize, _mapper.ConfigurationProvider);
            if (userId != null)
                result = await _productService.MarkLikedProductsAsync(result, (Guid)userId);

            return Ok(result);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductDto dto)
        {
            var newProduct = await _productService.CreateProductAsync(dto);

            return CreatedAtRoute("GetProduct", new { slug = newProduct.Slug }, _mapper.Map<ProductDetailDto>(newProduct));
        }
    }
}
