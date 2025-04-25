using AutoMapper;
using System.Linq.Expressions;

namespace API.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<TDto>> GetAllProjectedAsync<TDto>(IMapper mapper);
        Task<T> FindByIdAsync(int id);
        Task<TDto?> GetSingleProjectedAsync<TDto>(Expression<Func<T, bool>> predicate, IMapper mapper);
        Task<T> FindAsync(params object[] keyValues);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
    }
}
