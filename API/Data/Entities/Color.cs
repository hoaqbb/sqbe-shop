using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class Color
    {
        public Color()
        {
            ProductColors = new HashSet<ProductColor>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string ColorCode { get; set; } = null!;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }

        public virtual ICollection<ProductColor> ProductColors { get; set; }
    }
}
