namespace AzureCache.Redis.Lib.Interfaces
{
    public interface ICacheService
    {
        bool Exists(string key);
        bool Delete(string key);
        string GetValue(string key);
        void SetValue(string key, string value, int? expirySeconds = null);
    }
}
