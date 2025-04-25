﻿using API.DTOs.ColorDtos;

namespace API.DTOs.ProductDtos
{
    public class ProductListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string MainPhoto { get; set; }
        public string SubPhoto { get; set; }
        public ICollection<ColorDto> ProductColors { get; set; }
        public int Discount { get; set; }
        public bool isVisible { get; set; }
        public string Slug { get; set; } = null!;
        public string Category { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
