namespace API.DTOs.UserDtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public short Gender { get; set; }
        public string? Provider { get; set; }
        public string Role { get; set; }
        public bool IsAuthenticated { get; set; }
        public string CreateAt { get; set; }
    }
}
