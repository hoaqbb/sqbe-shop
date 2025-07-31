using API.Data.Entities;
using API.DTOs.ProductDtos;
using API.Helpers;
using API.Helpers.Params;
using AutoMapper;

namespace API.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<bool> IsProductLikedByCurrentUser(Guid userId, Guid productId);
        Task<List<Guid>> GetLikedProductIdsAsync(Guid userId, List<Guid> productIds);
        Task<ProductColor> AddProductColorAsync(ProductColor productColor);
        Task<ProductVariant> AddProductVariantAsync(ProductVariant productVariant);
        Task<ProductImage> AddProductImageAsync(ProductImage productImage);
        Task<List<ProductImage>> GetProductImagesAsync(Guid id);
    }
}
