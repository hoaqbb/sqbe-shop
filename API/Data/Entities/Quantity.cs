using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class Quantity
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public int? ProductColorId { get; set; }
        public int SizeId { get; set; }
        public int ProductId { get; set; }

        public virtual ProductColor? ProductColor { get; set; }
        public virtual Size Size { get; set; } = null!;
    }
}
