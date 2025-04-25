using API.Data.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace API.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly EcommerceDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(EcommerceDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            IQueryable<T> query = _dbSet;
            if(predicate != null)
                query = query.Where(predicate);
            return await query.CountAsync();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<T?> FindAsync(params object[] keyValues)
        {
            return await _dbSet.FindAsync(keyValues);
        }

        public async Task<T?> FindByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IReadOnlyList<TDto>> GetAllProjectedAsync<TDto>(IMapper mapper)
        {
            return await _dbSet
                .ProjectTo<TDto>(mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<TDto?> GetSingleProjectedAsync<TDto>(Expression<Func<T, bool>> predicate, IMapper mapper)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            var product = await query
                .ProjectTo<TDto?>(mapper.ConfigurationProvider)
                .SingleOrDefaultAsync();

            return product;
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
