using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class ProductColor
    {
        public ProductColor()
        {
            Quantities = new HashSet<Quantity>();
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ColorId { get; set; }

        public virtual Color Color { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public virtual ICollection<Quantity> Quantities { get; set; }
    }
}
