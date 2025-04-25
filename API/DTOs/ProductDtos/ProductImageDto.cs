namespace API.DTOs.ProductDtos
{
    public class ProductImageDto
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
        public bool IsSub { get; set; }
    }
}
