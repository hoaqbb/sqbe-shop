using API.Data.Entities;
using API.DTOs.CategoryDtos.cs;
using API.DTOs.ColorDtos;
using API.DTOs.OrderDtos;
using API.DTOs.ProductDtos;
using API.DTOs.PromotionDtos;
using API.DTOs.RevenueDto;
using API.DTOs.SizeDtos;
using API.DTOs.UserDtos;
using API.Helpers.Params;
using API.Interfaces;
using API.Specifications.AdminSpecifications;
using API.Specifications.ProductSpecifications;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ISizeService _sizeService;
        private readonly IColorService _colorService;
        private readonly IMapper _mapper;
        private readonly EcommerceDbContext _context;

        public AdminsController(IUnitOfWork unitOfWork, IProductService productService, ICategoryService categoryService, ISizeService sizeService, IColorService colorService, IMapper mapper, EcommerceDbContext context)
        {
            _unitOfWork = unitOfWork;
            _productService = productService;
            _categoryService = categoryService;
            _sizeService = sizeService;
            _colorService = colorService;
            _mapper = mapper;
            _context = context;
        }

        [HttpGet("products")]
        public async Task<ActionResult> GetProducts([FromQuery]AdminProductFilterParams param)
        {
            var spec = new AdminProductFilterSpecification(param);

            var result = await CreatePaginatedResult<Product, ProductListDto>(_unitOfWork.ProductRepository, spec, param.PageIndex, param.PageSize, _mapper.ConfigurationProvider);

            return Ok(result);
        }

        [HttpGet("product/{id:Guid}")]
        public async Task<ActionResult> GetProductById(Guid id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpGet("promotions")]
        public async Task<ActionResult> GetPromotions()
        {
            var result = await _context.Promotions
                .ProjectTo<PromotionDetailDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("categories")]
        public async Task<ActionResult> GetCategories()
        {
            var categories = await _categoryService.GetAllWithDetailsAsync();

            return Ok(categories);
        }

        [HttpGet("colors")]
        public async Task<ActionResult> GetColors()
        {
            var colors = await _colorService.GetAllWithDetailsAsync();

            return Ok(colors);
        }

        [HttpGet("sizes")]
        public async Task<ActionResult> GetSizes()
        {
            var sizes = await _sizeService.GetAllWithDetailsAsync();

            return Ok(sizes);
        }

        [HttpGet("overview")]
        public async Task<ActionResult<RevenueDto>> OverviewRevenue()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            //count number of orders in current month
            var monthlyOrders = await _context.Orders
                .Where(x => x.CreateAt.Month == currentMonth
                    && x.CreateAt.Year == currentYear
                    && x.Status <= 3)
                .CountAsync();

            //calculate revenue in current month
            //only orders with status = 3(delivery successful)
            var monthlyRevenue = await _context.Orders
                .Where(x => x.CreateAt.Month == currentMonth
                    && x.CreateAt.Year == currentYear
                    && x.Status == 3)
                .SumAsync(s => s.Amount - s.ShippingFee);

            //best selling products list
            //10 best selling products in first 20 orders
            var bestSellingProducts = await _context.OrderItems
                .Where(oi => _context.Orders
                    .Where(o => o.Status <= 3)
                    .OrderByDescending(o => o.CreateAt)
                    .Take(20)
                    .Select(o => o.Id)
                    .Contains(oi.OrderId))
                .GroupBy(p => new
                {
                    ProductId = p.ProductVariant.ProductId,
                    ProductName = p.ProductName,
                    ProductImage = p.ProductImageUrl
                })
                .Select(g => new
                {
                    ProductId = g.Key.ProductId,
                    ProductName = g.Key.ProductName,
                    ProductImage = g.Key.ProductImage,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(10)
                .ToListAsync();

            //list of orders waiting to be confirmed
            var unshippedOrders = await _context.Orders
                .Where(o => o.Status < 2)
                .OrderByDescending(o => o.CreateAt)
                .ProjectTo<OrderDto>(_mapper.ConfigurationProvider)
                .ToListAsync();

            //total products sold for each category of top 20 orders
            var categorySales = await _context.OrderItems
                .Where(oi => _context.Orders
                    .OrderByDescending(o => o.CreateAt)
                    .Take(20)
                    .Select(o => o.Id)
                    .Contains(oi.OrderId))
                .GroupBy(p => p.ProductVariant.Product.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    CategoryName = g.FirstOrDefault().ProductVariant.Product.Category.Name,
                    TotalSold = g.Sum(x => x.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .ToListAsync();

            var result = new RevenueDto();
            result.MonthlyOrders = monthlyOrders;
            result.MonthlyRevenue += monthlyRevenue;
            result.BestSellingProducts = bestSellingProducts;
            result.UnshippedOrders = unshippedOrders;
            result.CategorySales = categorySales;

            return Ok(result);
        }

        [HttpGet("revenue")]
        public async Task<ActionResult> CalculateRevenue([FromQuery] int? year)
        {
            var years = await _context.Orders
                .Where(x => x.Status == 3)
                .GroupBy(x => x.CreateAt.Year)
                .Select(p => p.Key)
                .ToListAsync();

            if (!years.Any()) years = new List<int> { 0 };

            int selectedYear = 0;
            if (year.HasValue) selectedYear = year.Value;
            else selectedYear = years.Last();

            List<int> months = Enumerable.Range(1, 12).ToList();

            var monthlyRevenue = await _context.Orders
                .Where(o => o.Status == 3 && o.CreateAt.Year == selectedYear)
                .GroupBy(o => o.CreateAt.Month)
                .Select(g => new
                {
                    Month = g.Key,
                    TotalOrder = g.Count(),
                    Revenue = g.Sum(o => o.Amount - o.ShippingFee)
                })
                .ToListAsync();

            var result = months.GroupJoin(
                    monthlyRevenue,
                    month => month,
                    revenue => revenue.Month,
                    (month, revenueGroup) => new
                    {
                        Month = month,
                        TotalOrder = revenueGroup.FirstOrDefault()?.TotalOrder ?? 0,
                        Revenue = revenueGroup.FirstOrDefault()?.Revenue ?? 0
                    }).ToList();

            var orderOverview = await _context.Orders
                    .Where(x => x.Status == 3 && x.CreateAt.Year == selectedYear)
                    .GroupBy(x => x.CreateAt.Year)
                    .Select(o => new
                    {
                        totalOrders = o.Count(),
                        totalSales = o.Sum(x => x.Amount - x.ShippingFee),
                        avgSalesPerOrders = o.Sum(x => x.Amount - x.ShippingFee) / o.Count(),
                        totalUnits = _context.OrderItems
                            .Where(x => x.Order.CreateAt.Year == selectedYear && x.Order.Status == 3)
                            .Sum(x => x.Quantity)
                    }).FirstOrDefaultAsync();

            return Ok(new
            {
                years = years,
                orderOverview = orderOverview,
                revenues = result
            });
        }
    }
}
