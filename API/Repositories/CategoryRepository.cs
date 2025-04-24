using API.Data.Entities;
using API.Interfaces;

namespace API.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        private readonly EcommerceDbContext _context;

        public CategoryRepository(EcommerceDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
