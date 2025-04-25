namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IAccountRepository AccountRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        Task<bool> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
