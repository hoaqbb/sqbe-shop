namespace API.DTOs.ProductDtos
{
    public class UpdateProductDto
    {
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string? Description { get; set; }
        public int Discount { get; set; }
        public int? CategoryId { get; set; }
        public List<int> ProductColors { get; set; }
        public List<int> ProductSizes { get; set; }
    }
}
