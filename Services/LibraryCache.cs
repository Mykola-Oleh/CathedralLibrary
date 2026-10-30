using Microsoft.Extensions.Caching.Memory;

namespace CathedralLibrary.Services;

public sealed class LibraryCache(IMemoryCache memory)
{
    private long _version;
    public long Version => Interlocked.Read(ref _version);
    public void Invalidate() => Interlocked.Increment(ref _version);

    public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory)
    {
        var version = Version;
        var cacheKey = $"library:{version}:{key}";
        if (memory.TryGetValue(cacheKey, out T? cached) && cached is not null)
            return cached;

        var result = await factory();
        // Do not cache stale data if a write happened during the query.
        if (version == Version)
            memory.Set(cacheKey, result, TimeSpan.FromSeconds(45));
        return result;
    }
}
