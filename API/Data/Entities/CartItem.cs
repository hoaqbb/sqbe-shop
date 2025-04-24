using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class CartItem
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public int ProductVariantId { get; set; }
        public string CartId { get; set; } = null!;

        public virtual Cart Cart { get; set; } = null!;
        public virtual ProductVariant ProductVariant { get; set; } = null!;
    }
}
