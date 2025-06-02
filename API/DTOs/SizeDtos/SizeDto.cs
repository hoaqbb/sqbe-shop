using System.ComponentModel.DataAnnotations;

namespace API.DTOs.SizeDtos
{
    public class SizeDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
