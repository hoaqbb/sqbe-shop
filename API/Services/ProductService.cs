using API.Data.Entities;
using API.DTOs.ProductDtos;
using API.Extensions;
using API.Helpers;
using API.Helpers.Params;
using API.Interfaces;
using API.Specifications.ProductSpecifications;

namespace API.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly EcommerceDbContext _context;
        private readonly ITokenService _tokenService;

        public ProductService(IUnitOfWork unitOfWork, EcommerceDbContext context, ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<PaginatedResult<ProductListDto>> MarkLikedProductsAsync(PaginatedResult<ProductListDto> result, Guid userId)
        {
            var productIds = result.Data.Select(p => p.Id).ToList();
            var likedProductIds = await _unitOfWork.ProductRepository
                .GetLikedProductIdsAsync((Guid)userId, productIds);

            foreach (var product in result.Data)
            {
                product.IsLikedByCurrentUser = likedProductIds.Contains(product.Id);
            }
            return result;
        }

        public async Task<PaginatedResult<ProductListDto>> SearchProducts(ProductSearchParams param, AutoMapper.IConfigurationProvider mapperConfig, HttpContext httpContext)
        {
            var userId = httpContext.GetUserIdFromTokenInsideCookie(_tokenService);

            var spec = new ProductSearchSpecification(param);
            var products = await _unitOfWork.ProductRepository
                .ListAsync<ProductListDto>(spec, mapperConfig);
            var count = await _unitOfWork.ProductRepository.CountAsync(spec);

            if (userId != null)
            {
                var productIds = products.Select(p => p.Id).ToList();
                var likedProductIds = await _unitOfWork.ProductRepository
                    .GetLikedProductIdsAsync((Guid)userId, productIds);

                foreach (var product in products)
                {
                    product.IsLikedByCurrentUser = likedProductIds.Contains(product.Id);
                }
            }

            return new PaginatedResult<ProductListDto>(products, count, param.PageIndex, param.PageSize);
        }
    }
}
