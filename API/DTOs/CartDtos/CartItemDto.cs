using API.DTOs.ProductDtos;

namespace API.DTOs.CartDtos
{
    public class CartItemDto
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public int Price { get; set; }
        public string Photo { get; set; }
        public int Quantity { get; set; }
        public int Discount { get; set; }
        public string Category { get; set; }
        public string Slug { get; set; }
        public ProductVariantDto ProductVariant { get; set; }
    }
}
