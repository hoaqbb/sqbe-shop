using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class Banner
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = null!;
        public string? LinkUrl { get; set; }
        public bool IsActive { get; set; }
        public string PublicId { get; set; } = null!;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public short DisplayType { get; set; }
    }
}
