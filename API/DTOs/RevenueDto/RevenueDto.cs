namespace API.DTOs.RevenueDto
{
    public class RevenueDto
    {
        public decimal MonthlyRevenue { get; set; } = 0;
        public int MonthlyOrders { get; set; } = 0;
        public dynamic BestSellingProducts { get; set; } = null;
        public dynamic UnshippedOrders { get; set; } = null;
        public dynamic CategorySales { get; set; } = null;
    }
}
