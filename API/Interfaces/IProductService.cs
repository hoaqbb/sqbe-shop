using API.DTOs.ProductDtos;
using API.Helpers;
using API.Helpers.Params;

namespace API.Interfaces
{
    public interface IProductService
    {
        Task<PaginatedResult<ProductListDto>> SearchProducts(
            ProductSearchParams param, 
            AutoMapper.IConfigurationProvider mapperConfig, 
            HttpContext httpContext
            );
        Task<Product> CreateProductAsync(CreateProductDto dto);
        Task<PaginatedResult<ProductListDto>> MarkLikedProductsAsync(PaginatedResult<ProductListDto> result, Guid userId);
    }
}
