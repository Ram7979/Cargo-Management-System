namespace CMS.FleetService.Application.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
    Task RemoveAsync(string key);
    Task<IEnumerable<T>> GetByPatternAsync<T>(string pattern);
}
