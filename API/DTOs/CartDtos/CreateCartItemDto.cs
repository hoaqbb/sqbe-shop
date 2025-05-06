using System.ComponentModel.DataAnnotations;

namespace API.DTOs.CartDtos
{
    public class CreateCartItemDto
    {
        [Required]
        public int ProductVariantId { get; set; }
    }
}
