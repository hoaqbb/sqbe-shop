namespace API.Helpers.Params
{
    public class ProductFilterParams : PaginationParams
    {
        public string? Category { get; set; }
        public List<int>? Colors { get; set; }
        public List<int>? Sizes { get; set; }
        public int? PriceFrom { get; set; }
        public int? PriceTo { get; set; }
        public bool Promotion { get; set; } = false;
        public string? Sort { get; set; }
    }
}
