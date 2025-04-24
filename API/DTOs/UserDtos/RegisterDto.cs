using System.ComponentModel.DataAnnotations;

namespace API.DTOs.UserDtos
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(20)]
        public string Lastname { get; set; }
        [Required]
        [MaxLength(20)]
        public string Firstname { get; set; }
        [Required]
        public byte Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [MinLength(8)]
        public string Password { get; set; }
    }
}
