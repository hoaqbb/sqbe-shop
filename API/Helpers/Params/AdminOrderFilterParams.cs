using API.Data.Enums;

namespace API.Helpers.Params
{
    public class AdminOrderFilterParams : PaginationParams
    {
        public string? Sort { get; set; }
        public short? Status { get; set; }
        public decimal? AmountFrom { get; set; }
        public decimal? AmountTo { get; set; }
        public bool? isDiscounted { get; set; }
        public string? PaymentMethod { get; set; }
    }
}
