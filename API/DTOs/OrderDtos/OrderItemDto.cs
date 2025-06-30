namespace API.DTOs.OrderDtos
{
    public class OrderItemDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductColor { get; set; }
        public string? ProductSize { get; set; }
        public decimal Price { get; set; }
        public short Discount { get; set; }
        public string? ProductImageUrl { get; set; }
    }
}
