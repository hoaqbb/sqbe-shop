using API.Data.Entities;
using API.DTOs.ProductDtos;
using API.Extensions;
using API.Helpers.Params;
using API.Interfaces;
using API.Specifications.ProductSpecifications;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class ProductsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly EcommerceDbContext _context;
        private readonly IImageService _imageService;

        public ProductsController(IUnitOfWork unitOfWork, ITokenService tokenService, IProductService productService, IMapper mapper, EcommerceDbContext context, IImageService imageService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _productService = productService;
            _mapper = mapper;
            _context = context;
            _imageService = imageService;
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

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            if(await _productService.DeleteProductAsync(id))
                return NoContent();

            return BadRequest("Problem deleting the product!");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}/status")]
        public async Task<IActionResult> UpdateProductStatus(Guid id)
        {
            if (await _productService.UpdateProductStatusAsync(id))
                return NoContent();

            return BadRequest("Failed to update product status!");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:Guid}")]
        public async Task<ActionResult> UpdateProduct(Guid id, UpdateProductDto updateProductDto)
        {
            var isProductExisted = await _unitOfWork.ProductRepository
                    .ExistsAsync(p => p.Id == id);
            if (!isProductExisted) return NotFound();

            if (await _productService.UpdateProductAsync(id, updateProductDto))
                return Ok();
            return BadRequest();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{productId}/delete-image/{imageId}")]
        public async Task<ActionResult> DeleteImage(Guid productId, int imageId)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == productId);

            var image = await _context.ProductImages
                .Include(x => x.Product)
                .Where(x => x.Id == imageId && x.Product.Id == productId)
                .SingleOrDefaultAsync(x => x.Id == imageId);

            if (image is null) return NotFound();

            if (image.IsMain) return BadRequest("You cannot delete your main photo!");
            if (image.IsSub) return BadRequest("You cannot delete your sub photo!");

            if (image.PublicId != null)
            {
                var result = await _imageService.DeleteImageAsync(image.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            product.ProductImages.Remove(image);

            if (await _unitOfWork.SaveChangesAsync()) return Ok();

            return BadRequest("Failed to delete the image!");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{productId}/set-main-image")]
        public async Task<ActionResult> SetMainImage(Guid productId, [FromQuery] int imageId)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == productId);

            var image = product.ProductImages.FirstOrDefault(x => x.Id == imageId);

            if (image.IsMain) return BadRequest("This is already main image");

            var currentMain = product.ProductImages.FirstOrDefault(x => x.IsMain);

            if (currentMain != null) currentMain.IsMain = false;
            image.IsMain = true;

            if (await _unitOfWork.SaveChangesAsync()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{productId}/set-sub-image")]
        public async Task<ActionResult> SetSubImage(Guid productId, [FromQuery] int imageId)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.Id == productId);

            if (product is null) return NotFound();

            var image = product.ProductImages.FirstOrDefault(x => x.Id == imageId);

            if (image.IsSub) return BadRequest("This is already sub image");

            var currentSub = product.ProductImages.FirstOrDefault(x => x.IsSub);

            if (currentSub != null) currentSub.IsSub = false;
            image.IsSub = true;

            if (await _unitOfWork.SaveChangesAsync()) return NoContent();

            return BadRequest("Failed to set main photo");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{productId}/add-product-images")]
        public async Task<ActionResult<List<ProductImageDto>>> AddProductImages(
            Guid productId, [FromForm] IFormFile[] imageFiles)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .Include(p => p.ProductColors)
                .SingleOrDefaultAsync(x => x.Id == productId);

            var result = new List<ImageUploadResult>();

            try
            {
                await _context.Database.BeginTransactionAsync();
                result.AddRange(await _imageService.AddMultipleImagesAsync(imageFiles));

                var listProductImages = new List<ProductImageDto>();
                foreach (var item in result)
                {
                    if (item.Error != null)
                    {
                        await _context.Database.RollbackTransactionAsync();
                        return BadRequest(item.Error.Message);
                    }

                    var image = new ProductImage
                    {
                        ImageUrl = item.SecureUrl.AbsoluteUri,
                        PublicId = item.PublicId,
                        ProductId = productId
                    };

                    if (product.ProductImages.Count == 0)
                    {
                        image.IsMain = true;
                    }
                    if (product.ProductImages.Count == 1)
                    {
                        image.IsSub = true;
                    }

                    product.ProductImages.Add(image);
                    _context.SaveChanges();

                    listProductImages.Add(_mapper.Map<ProductImageDto>(image));
                }

                if (listProductImages.Count() > 0)
                {
                    await _context.Database.CommitTransactionAsync();
                    return Ok(listProductImages);
                }
                return BadRequest("Problem adding image!");
            }
            catch (Exception ex)
            {
                await _context.Database.RollbackTransactionAsync();
                return BadRequest(ex.Message);
            }
        }
    }
}
