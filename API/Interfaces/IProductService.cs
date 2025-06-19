using API.Data.Entities;
using API.DTOs.ProductDtos;
using API.Helpers;
using API.Helpers.Params;

namespace API.Interfaces
{
    public interface IProductService
    {
        Task<Product> CreateProductAsync(CreateProductDto dto);
        Task<bool> DeleteProductAsync(Guid id);
        Task<PaginatedResult<ProductListDto>> MarkLikedProductsAsync(PaginatedResult<ProductListDto> result, Guid userId);
        Task<ProductDetailDto> GetProductByIdAsync(Guid id);
        Task<bool> UpdateProductStatusAsync(Guid id);
        Task<bool> UpdateProductAsync(Guid productId, UpdateProductDto updateProductDto);
    }
}
