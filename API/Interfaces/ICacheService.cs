namespace API.Interfaces
{
    public interface ICacheService
    {
        Task<T> GetDataAsync<T>(string key);
        Task<T> SetDataAsync<T>(string key, T value);
        Task<bool> RemoveDataAsync(string key);
    }
}
