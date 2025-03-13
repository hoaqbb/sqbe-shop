namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}
