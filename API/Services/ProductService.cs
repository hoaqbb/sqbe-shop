using API.Data.Entities;
using API.DTOs.ProductDtos;
using API.Extensions;
using API.Helpers;
using API.Helpers.Params;
using API.Interfaces;
using API.Specifications.ProductSpecifications;
using Slugify;

namespace API.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageService _imageService;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IUnitOfWork unitOfWork, IImageService imageService, ILogger<ProductService> logger)
        {
            _unitOfWork = unitOfWork;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<Product> CreateProductAsync(CreateProductDto createProductDto)
        {
            try
            {
                // Begin transaction
                await _unitOfWork.BeginTransactionAsync();
                var createProduct = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = createProductDto.Name,
                    Slug = GenerateSlug(createProductDto.Name),
                    Price = createProductDto.Price,
                    Description = createProductDto.Description,
                    Discount = createProductDto.Discount,
                    CategoryId = createProductDto.CategoryId,
                    IsVisible = false
                };

                await _unitOfWork.ProductRepository.AddAsync(createProduct);

                // Add product colors to db
                var productColors = await AddProductColorsAsync(createProduct.Id, createProductDto.ProductColors);

                if (!await _unitOfWork.SaveChangesAsync()) 
                    throw new Exception("Falied to add product colors!");

                // Add product variants
                await AddProductVariantsAsync(createProduct.Id, productColors, createProductDto.ProductSizes);

                // Add main image
                await AddProductImagesAsync(
                    createProduct.Id,
                    createProductDto.MainImage,
                    createProductDto.SubImage,
                    createProductDto.ProductImages
                );

                // Save changes
                if (!await _unitOfWork.SaveChangesAsync())
                {
                    throw new ApplicationException("Failed to create product!");
                }

                // Commit transaction
                await _unitOfWork.CommitAsync();

                return createProduct;
            }
            catch (Exception ex)
            {
                // Rollback on error
                await _unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            var product = await _unitOfWork.ProductRepository.FindAsync(id);
            if (product == null) return false;

            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var productImages = await _unitOfWork.ProductRepository.GetProductImagesAsync(product.Id);

                if (productImages.Count() > 0)
                {
                    var publicIds = productImages.Select(x => x.PublicId).ToList();
                    var deleteResults = await _imageService.DeleteMultipleImagesAsync(publicIds!);

                    var failedImages = deleteResults.Where(r => r.Error != null).ToList();
                    if (failedImages.Any())
                    {
                        foreach (var fail in failedImages)
                        {
                            _logger.LogWarning($"Failed to delete image: {fail.Error.Message}");
                        }
                    }
                }
                _unitOfWork.ProductRepository.Delete(product);
                if (!await _unitOfWork.SaveChangesAsync())
                {
                    throw new ApplicationException("Failed to delete product!");
                }

                await _unitOfWork.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                throw;
            }
            
        }

        private async Task<List<ProductColor>> AddProductColorsAsync(Guid productId, List<int> colorIds)
        {
            var productColors = new List<ProductColor>();

            foreach (var colorId in colorIds)
            {
                var productColor = new ProductColor
                {
                    ProductId = productId,
                    ColorId = colorId
                };

                var addedColor = await _unitOfWork.ProductRepository.AddProductColorAsync(productColor);
                productColors.Add(addedColor);
            }

            return productColors;
        }

        public async Task<bool> UpdateProductStatusAsync(Guid id)
        {
            var product = await _unitOfWork.Repository<Product>().FindAsync(id);

            if (product is null) return false;

            product.IsVisible = !product.IsVisible;
            _unitOfWork.Repository<Product>().Update(product);

            if (await _unitOfWork.SaveChangesAsync())
                return true;

            return false;
        }

        private async Task AddProductVariantsAsync(Guid productId, List<ProductColor> productColors, List<int> sizeIds)
        {
            foreach (var sizeId in sizeIds)
            {
                foreach (var productColor in productColors)
                {
                    var variant = new ProductVariant
                    {
                        ProductId = productId,
                        ProductColorId = productColor.Id,
                        SizeId = sizeId,
                        Quantity = 0
                    };

                    await _unitOfWork.ProductRepository.AddProductVariantAsync(variant);
                }
            }
        }

        private async Task AddProductImagesAsync(Guid productId, IFormFile mainImage, IFormFile subImage, IFormFile[] productImages)
        {
            // Add main image
            var mainImgResult = await _imageService.AddImageAsync(mainImage);
            if (mainImgResult.Error == null)
            {
                await _unitOfWork.ProductRepository.AddProductImageAsync(new ProductImage
                {
                    ProductId = productId,
                    IsMain = true,
                    ImageUrl = mainImgResult.SecureUrl.AbsoluteUri,
                    PublicId = mainImgResult.PublicId
                });
            }
            else
            {
                throw new ApplicationException("Failed to upload main product image");
            }

            // Add sub image
            if (subImage != null)
            {
                var subImgResult = await _imageService.AddImageAsync(subImage);
                if (subImgResult.Error == null)
                {
                    await _unitOfWork.ProductRepository.AddProductImageAsync(new ProductImage
                    {
                        ProductId = productId,
                        IsSub = true,
                        ImageUrl = subImgResult.SecureUrl.AbsoluteUri,
                        PublicId = subImgResult.PublicId
                    });
                }
            }

            // Add additional images
            if (productImages != null && productImages.Length > 0)
            {
                var otherImgResults = await _imageService.AddMultipleImagesAsync(productImages);
                foreach (var imgResult in otherImgResults)
                {
                    if (imgResult.Error == null)
                    {
                        await _unitOfWork.ProductRepository.AddProductImageAsync(new ProductImage
                        {
                            ProductId = productId,
                            ImageUrl = imgResult.SecureUrl.AbsoluteUri,
                            PublicId = imgResult.PublicId
                        });
                    }
                }
            }
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

        private string GenerateSlug(string title)
        {
            var randomNumber = new Random().Next(1000000);
            return new SlugHelper()
                .GenerateSlug(title + " " + randomNumber.ToString());
        }
    }
}
