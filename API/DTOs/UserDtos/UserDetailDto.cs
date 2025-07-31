using API.DTOs.OrderDtos;

namespace API.DTOs.UserDtos
{
    public class UserDetailDto
    {
        public Guid Id { get; set; }
        public string Lastname { get; set; } = null!;
        public string Firstname { get; set; } = null!;
        public DateTime? DateOfBirth { get; set; }
        public string Email { get; set; } = null!;
        public short Gender { get; set; }
        public string? Provider { get; set; }
        public DateTime? CreateAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public string Role { get; set; } = null!;
        public bool IsAuthenticated { get; set; }

        public List<OrderDto> UserOrders { get; set; }
    }
}
