using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class ProductColor
    {
        public ProductColor()
        {
            ProductVariants = new HashSet<ProductVariant>();
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ColorId { get; set; }

        public virtual Color Color { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
    }
}
