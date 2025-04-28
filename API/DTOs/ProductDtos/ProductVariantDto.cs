using API.DTOs.ColorDtos;

namespace API.DTOs.ProductDtos
{
    public class ProductVariantDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string Color { get; set; } = null!;
        public string ColorCode { get; set; } = null!;
        public string Size { get; set; } = null!;
    }
}
