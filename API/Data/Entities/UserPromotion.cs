using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class UserPromotion
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public int PromotionId { get; set; }
        public DateTime? UsedAt { get; set; }

        public virtual Promotion Promotion { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
