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
        private readonly EcommerceDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IImageService _imageService;

        public ProductService(IUnitOfWork unitOfWork, EcommerceDbContext context, ITokenService tokenService, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _tokenService = tokenService;
            _imageService = imageService;
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

        private string GenerateSlug(string title)
        {
            var randomNumber = new Random().Next(1000000);
            return new SlugHelper()
                .GenerateSlug(title + " " + randomNumber.ToString());
        }
    }
}
