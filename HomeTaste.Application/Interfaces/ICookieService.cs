namespace HomeTaste.Application.Interfaces
{
    public interface ICookieService
    {
        Task SetCookieAsync<T>(string key, T value, DateTime expiresAt);
        Task<T?> GetCookieAsync<T>(string key);
        Task RemoveCookieAsync(string key);
    }
}
