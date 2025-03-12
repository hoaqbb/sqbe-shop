using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class UserLike
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public DateTime? CreateAt { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
