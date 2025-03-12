using System;
using System.Collections.Generic;
using System.Collections;

namespace API.Data.Entities
{
    public partial class Payment
    {
        public Payment()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public int OrderId { get; set; }
        public int? UserId { get; set; }
        public string Method { get; set; } = null!;
        public string? Provider { get; set; }
        public BitArray Status { get; set; } = null!;
        public decimal Amount { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? TransactionId { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
