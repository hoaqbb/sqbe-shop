namespace API.DTOs.ProductDtos
{
    public class ProductVariantDto
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public string Color { get; set; }
        public string ColorCode { get; set; }
        public string Size { get; set; } = null!;
    }
}
