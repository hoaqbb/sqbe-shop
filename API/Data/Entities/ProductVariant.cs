﻿using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class ProductVariant
    {
        public ProductVariant()
        {
            CartItems = new HashSet<CartItem>();
        }

        public int Id { get; set; }
        public int Quantity { get; set; }
        public int? ProductColorId { get; set; }
        public int SizeId { get; set; }
        public Guid ProductId { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual ProductColor? ProductColor { get; set; }
        public virtual Size Size { get; set; } = null!;
        public virtual ICollection<CartItem> CartItems { get; set; }
    }
}
