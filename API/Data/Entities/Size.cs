using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class Size
    {
        public Size()
        {
            ProductVariants = new HashSet<ProductVariant>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<ProductVariant> ProductVariants { get; set; }
    }
}
