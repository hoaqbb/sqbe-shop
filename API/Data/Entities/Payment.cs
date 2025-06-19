using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class Payment
    {
        public Payment()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Method { get; set; } = null!;
        public string? Provider { get; set; }
        public decimal Amount { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string? TransactionId { get; set; }
        public Guid? UserId { get; set; }
        public Guid OrderId { get; set; }
        public bool Status { get; set; }
        public string CurrencyCode { get; set; } = null!;

        public virtual Order Order { get; set; } = null!;
        public virtual User? User { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}
