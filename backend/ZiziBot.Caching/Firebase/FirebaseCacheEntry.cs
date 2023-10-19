namespace ZiziBot.Caching.Firebase;

public class FirebaseCacheEntry<T>
{
    public string CacheKey { get; set; }
    public T? Value { get; set; }
    public DateTime Expiry { get; set; }
}