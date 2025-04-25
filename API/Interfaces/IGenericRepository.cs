using AutoMapper;
using System.Linq.Expressions;

namespace API.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<TDto>> GetAllProjectedAsync<TDto>(AutoMapper.IConfigurationProvider config);
        Task<T> FindByIdAsync(int id);
        Task<T> FindAsync(params object[] keyValues);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
    }
}
