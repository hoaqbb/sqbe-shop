using API.Data.Entities;
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
        private readonly IMapper _mapper;

        public AdminsController(IUnitOfWork unitOfWork, IProductService productService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet("products")]
        public async Task<ActionResult> GetProducts([FromQuery]AdminProductFilterParams param)
        {
            var spec = new AdminProductFilterSpecification(param);

            var result = await CreatePaginatedResult<Product, ProductListDto>(_unitOfWork.ProductRepository, spec, param.PageIndex, param.PageSize, _mapper.ConfigurationProvider);

            return Ok(result);
        }
    }
}
