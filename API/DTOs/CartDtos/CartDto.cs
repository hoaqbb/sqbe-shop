namespace API.DTOs.CartDtos
{
    public class CartDto
    {
        public string Id { get; set; } = string.Empty;
        public List<CartItemDto> CartItems { get; set; } = new();
    }
}
