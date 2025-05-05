using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class Order
    {
        public Order()
        {
            OrderItems = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public string? Note { get; set; }
        public decimal Amount { get; set; }
        public short Status { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public int? PaymentId { get; set; }
        public string Address { get; set; } = null!;
        public string Fullname { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public int ShippingFee { get; set; }
        public string Email { get; set; } = null!;
        public short DeliveryMethod { get; set; }
        public Guid? UserId { get; set; }

        public virtual Payment? Payment { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}
