using System.ComponentModel.DataAnnotations;

namespace API.DTOs.OrderDtos
{
    public class UpdateOrderStatus
    {
        [Range(0, 4)]
        public short Status { get; set; }
    }
}
