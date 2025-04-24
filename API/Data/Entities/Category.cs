using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? Slug { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
