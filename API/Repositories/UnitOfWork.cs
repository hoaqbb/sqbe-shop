﻿using API.Data.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;

namespace API.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly EcommerceDbContext _context;
        private IDbContextTransaction _transaction;
        private readonly Dictionary<Type, object> _repositories;
        private bool _disposedValue;

        public UnitOfWork(EcommerceDbContext context)
        {
            _context = context;
            _repositories = new Dictionary<Type, object>();
        }

        public IAccountRepository AccountRepository => new AccountRepository(_context);
        public ICategoryRepository CategoryRepository =>  new CategoryRepository(_context);
        public IProductRepository ProductRepository => new ProductRepository(_context);
        public ICartRepository CartRepository => new CartRepository(_context);

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);

            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] = new GenericRepository<T>(_context);
            }

            return (IGenericRepository<T>)_repositories[type];
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction is not null)
                throw new InvalidOperationException("A transaction has already been started.");
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitAsync()
        {
            if (_transaction is null)
                throw new InvalidOperationException("A transaction has not been started.");

            try
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null!;
            }
            catch (Exception ex)
            {
                if(_transaction is not null)
                    await _transaction.RollbackAsync();
                throw ex;
            }
        }

        public async Task RollbackAsync()
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null!;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                    _transaction?.Dispose();
                }

                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        
    }
}
