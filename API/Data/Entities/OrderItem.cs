using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class OrderItem
    {
        public int Id { get; set; }
        public int? ProductVariantId { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; } = null!;
        public string? ProductColor { get; set; }
        public string? ProductSize { get; set; }
        public decimal Price { get; set; }
        public short Discount { get; set; }
        public string? ProductImageUrl { get; set; }
        public Guid OrderId { get; set; }

        public virtual Order Order { get; set; } = null!;
        public virtual ProductVariant? ProductVariant { get; set; }
    }
}
