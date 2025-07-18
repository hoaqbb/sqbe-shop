﻿using AutoMapper;
using System.Linq.Expressions;

namespace API.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<IReadOnlyList<TDto>> GetAllProjectedAsync<TDto>(AutoMapper.IConfigurationProvider config);
        Task<IReadOnlyList<TDto>> GetAllProjectedAsync<TDto>(Expression<Func<T, bool>> predicate, AutoMapper.IConfigurationProvider config);
        Task<T?> FindByIdAsync(int id);
        Task<TDto?> GetSingleProjectedAsync<TDto>(Expression<Func<T, bool>> predicate, AutoMapper.IConfigurationProvider config);
        Task<T?> FindAsync(params object[] keyValues);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        Task<int> CountAsync(ISpecification<T> specifications);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<T> spec, AutoMapper.IConfigurationProvider config);
        Task<T?> GetSingleOrDefaultAsync(Expression<Func<T, bool>>? predicate = null);
        Task<bool> ExistsAsync(Expression<Func<T, bool>>? predicate = null);
    }
}
