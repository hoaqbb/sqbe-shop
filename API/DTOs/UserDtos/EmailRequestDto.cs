using System.ComponentModel.DataAnnotations;

namespace API.DTOs.UserDtos
{
    public class EmailRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
