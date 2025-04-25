using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class ProductImage
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public bool IsMain { get; set; }
        public bool IsSub { get; set; }
        public string? PublicId { get; set; }
        public Guid? ProductId { get; set; }

        public virtual Product? Product { get; set; }
    }
}
