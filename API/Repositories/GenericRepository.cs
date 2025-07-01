using API.Data.Entities;
using API.Interfaces;
using API.Specifications;
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

        public async Task<int> CountAsync(ISpecification<T> specifications)
        {
            var query = _dbSet.AsQueryable();
            query = specifications.ApplyCriteria(query);

            return await query.CountAsync();
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>>? predicate = null)
        {
            return await _dbSet.AnyAsync(predicate);
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

        public async Task<IReadOnlyList<TDto>> GetAllProjectedAsync<TDto>(AutoMapper.IConfigurationProvider config)
        {
            return await _dbSet
                .ProjectTo<TDto>(config)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<TDto>> GetAllProjectedAsync<TDto>(Expression<Func<T, bool>> predicate, AutoMapper.IConfigurationProvider config)
        {
            return await _dbSet
                .Where(predicate)
                .ProjectTo<TDto>(config)
                .ToListAsync();
        }

        public async Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>>? predicate = null)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            var product = await query
                .SingleOrDefaultAsync();

            return product;
        }

        public async Task<TDto?> GetSingleProjectedAsync<TDto>(Expression<Func<T, bool>> predicate, AutoMapper.IConfigurationProvider config)
        {
            IQueryable<T> query = _dbSet.Where(predicate);

            var product = await query
                .ProjectTo<TDto?>(config)
                .SingleOrDefaultAsync();

            return product;
        }

        public async Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }

        public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T> spec, AutoMapper.IConfigurationProvider config)
        {
            return await ApplySpecification<TResult>(spec, config).ToListAsync();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_dbSet.AsQueryable(), spec);
        }

        private IQueryable<TResult> ApplySpecification<TResult>(ISpecification<T> spec, AutoMapper.IConfigurationProvider config)
        {
            return SpecificationEvaluator<T>.GetQuery<TResult>(_dbSet.AsQueryable(), spec, config);
        }
    }
}
