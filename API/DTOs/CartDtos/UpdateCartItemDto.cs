using System.ComponentModel.DataAnnotations;

namespace API.DTOs.CartDtos
{
    public class UpdateCartItemDto
    {
        [Required]
        [Range(1, int.MaxValue, 
            ErrorMessage = "Quantity must be a non-negative number and greater than 0.")]
        public int Quantity { get; set; }
    }
}
