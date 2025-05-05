using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class UserLike
    {
        public int Id { get; set; }
        public DateTime? CreateAt { get; set; }
        public Guid ProductId { get; set; }
        public Guid UserId { get; set; }

        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
