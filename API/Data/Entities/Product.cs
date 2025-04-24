using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class Product
    {
        public Product()
        {
            ProductColors = new HashSet<ProductColor>();
            ProductImages = new HashSet<ProductImage>();
            UserLikes = new HashSet<UserLike>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Price { get; set; }
        public string? Description { get; set; }
        public int Discount { get; set; }
        public string Slug { get; set; } = null!;
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? CategoryId { get; set; }
        public bool IsVisible { get; set; }
        public string? ModifiedBy { get; set; }

        public virtual Category? Category { get; set; }
        public virtual ICollection<ProductColor> ProductColors { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
        public virtual ICollection<UserLike> UserLikes { get; set; }
    }
}
