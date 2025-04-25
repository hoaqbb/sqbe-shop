using API.Data.Entities;
using API.DTOs.ProductDtos;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly EcommerceDbContext _context;

        public ProductRepository(EcommerceDbContext context) : base(context)
        {
            _context = context;
        }
        
    }
}
