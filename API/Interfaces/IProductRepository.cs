using API.Data.Entities;
using API.DTOs.ProductDtos;
using AutoMapper;

namespace API.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<bool> IsProductLikedByCurrentUser(int userId, Guid productId);
    }
}
