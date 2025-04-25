using API.Data.Entities;
using API.DTOs.ProductDtos;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductsController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> GetProducts()
        {
            var products = await _unitOfWork.ProductRepository
                .GetAllProjectedAsync<ProductListDto>(_mapper);

            return Ok(products);
        }

        [HttpGet("{slug}")]
        public async Task<ActionResult> GetProductBySlug(string slug)
        {
            var product = await _unitOfWork.ProductRepository
                .GetSingleProjectedAsync<ProductDetailDto>(
                    (x => x.Slug == slug && x.IsVisible == true), 
                    _mapper
                );

            if (product == null) return NotFound("Product not found!");

            return Ok(product);
        }
    }
}
