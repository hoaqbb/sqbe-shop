using System.ComponentModel.DataAnnotations;

namespace API.DTOs.UserDtos
{
    public class ExternalAuthDto
    {
        [Required]
        public string Token { get; set; }
    }
}
