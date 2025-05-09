using System.ComponentModel.DataAnnotations;

namespace API.DTOs.ProductDtos
{
    public class CreateProductDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        [Range(0, int.MaxValue,
            ErrorMessage = "Price must be a non-negative number.")]
        public int Price { get; set; }
        public string? Description { get; set; }
        [Range(0, 100,
            ErrorMessage = "Discount must be between 0-100.")]
        public int Discount { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public List<int> ProductColors { get; set; }
        [Required]

        public List<int> ProductSizes { get; set; }
        [Required]
        public IFormFile MainImage { get; set; }
        [Required]
        public IFormFile SubImage { get; set; }
        public IFormFile[] ProductImages { get; set; }
    }
}
