using System;
using System.Collections.Generic;

namespace API.Data.Entities
{
    public partial class User
    {
        public User()
        {
            Carts = new HashSet<Cart>();
            Orders = new HashSet<Order>();
            Payments = new HashSet<Payment>();
            UserLikes = new HashSet<UserLike>();
        }

        public int Id { get; set; }
        public string Lastname { get; set; } = null!;
        public string Firstname { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; } = null!;
        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;
        public short Gender { get; set; }
        public string? Provider { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? TokenExpiryTime { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string Role { get; set; } = null!;
        public bool IsAuthenticated { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
        public virtual ICollection<UserLike> UserLikes { get; set; }
    }
}
