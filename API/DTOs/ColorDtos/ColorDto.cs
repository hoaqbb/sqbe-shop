using System.ComponentModel.DataAnnotations;

namespace API.DTOs.ColorDtos
{
    public class ColorDto
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string ColorCode { get; set; }
    }
}
