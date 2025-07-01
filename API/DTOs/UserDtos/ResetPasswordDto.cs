using System.ComponentModel.DataAnnotations;

namespace API.DTOs.UserDtos
{
    public class ResetPasswordDto
    {
        [Required]
        public string NewPassword { get; set; }
    }
}
