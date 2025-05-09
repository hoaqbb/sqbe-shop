using API.Data.Entities;
using API.DTOs.ProductDtos;
using API.Helpers;
using API.Helpers.Params;
using API.Interfaces;
using API.Specifications;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly EcommerceDbContext _context;

        public ProductRepository(EcommerceDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProductColor> AddProductColorAsync(ProductColor productColor)
        {
            var result = await _context.ProductColors.AddAsync(productColor);
            return result.Entity;
        }

        public async Task<ProductImage> AddProductImageAsync(ProductImage productImage)
        {
            var result = await _context.ProductImages.AddAsync(productImage);
            return result.Entity;
        }

        public async Task<ProductVariant> AddProductVariantAsync(ProductVariant productVariant)
        {
            var result = await _context.ProductVariants.AddAsync(productVariant);
            return result.Entity;
        }

        public async Task<List<Guid>> GetLikedProductIdsAsync(Guid userId, List<Guid> productIds)
        {
            var likedProductIds = await _context.UserLikes
                .Where(x => x.UserId == userId && productIds.Contains(x.ProductId))
                .Select(x => x.ProductId)
                .ToListAsync();

            return likedProductIds;
        }

        public async Task<bool> IsProductLikedByCurrentUser(Guid userId, Guid productId)
        {
            var isLiked = await _context.UserLikes
                .AnyAsync(x => x.UserId == userId && x.ProductId == productId);

            return isLiked;
        }
    }
}
