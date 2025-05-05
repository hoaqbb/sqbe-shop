using API.Data.Entities;
using API.Interfaces;

namespace API.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        public CartRepository(EcommerceDbContext context) : base(context)
        {
        }
    }
}
