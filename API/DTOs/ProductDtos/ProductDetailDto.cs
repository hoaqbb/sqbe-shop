using API.DTOs.CategoryDtos.cs;

namespace API.DTOs.ProductDtos
{
    public class ProductDetailDto
    {
        public string Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string? Description { get; set; }
        public int? Discount { get; set; }
        public string Slug { get; set; } = null!;
        public CategoryDto Category { get; set; }
        public ICollection<ProductImageDto> ProductImages { get; set; }
        public ICollection<ProductVariantDto> ProductVariants { get; set; }
    }
}
