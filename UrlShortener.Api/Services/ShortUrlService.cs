using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;
using UrlShortener.Api.Models;

public interface IShortUrlService
{
    Task<ShortUrl> CreateAsync(string originalUrl, string? userId);
    Task<string?> ResolveAsync(string code);
    Task<IEnumerable<ShortUrl>> GetByUserAsync(string? userId);
}

public class ShortUrlService : IShortUrlService
{
    private readonly ApplicationDbContext _db;
    private readonly IConnectionMultiplexer _redis;
    private readonly IDatabase _cache;

    public ShortUrlService(ApplicationDbContext db, IConnectionMultiplexer redis)
    {
        _db = db;
        _redis = redis;
        _cache = _redis.GetDatabase();
    }

    public async Task<ShortUrl> CreateAsync(string originalUrl, string? userId)
    {
        // validate URL
        if (!Uri.TryCreate(originalUrl, UriKind.Absolute, out _))
            throw new ArgumentException("Invalid URL");

        // generate unique code (6 chars base62)
        string code;
        do
        {
            code = GenerateCode(6);
        } while (await _db.ShortUrls.AnyAsync(s => s.Code == code));

        var shortUrl = new ShortUrl { Code = code, OriginalUrl = originalUrl, CreatedByUserId = userId };
        _db.ShortUrls.Add(shortUrl);
        await _db.SaveChangesAsync();

        // cache it
        await _cache.StringSetAsync($"short:{code}", originalUrl, TimeSpan.FromDays(30));
        return shortUrl;
    }

    public async Task<string?> ResolveAsync(string code)
    {
        var cached = await _cache.StringGetAsync($"short:{code}");
        if (cached.HasValue) return cached.ToString();

        var entry = await _db.ShortUrls.FirstOrDefaultAsync(s => s.Code == code);
        if (entry == null) return null;

        entry.HitCount++;
        await _db.SaveChangesAsync();

        await _cache.StringSetAsync($"short:{code}", entry.OriginalUrl, TimeSpan.FromDays(30));
        return entry.OriginalUrl;
    }

    public async Task<IEnumerable<ShortUrl>> GetByUserAsync(string? userId)
    {
        return await _db.ShortUrls.Where(s => s.CreatedByUserId == userId).ToListAsync();
    }

    private static string GenerateCode(int length)
    {
        const string chars = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var sb = new StringBuilder();
        var rng = RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);
        for (int i = 0; i < length; i++)
            sb.Append(chars[bytes[i] % chars.Length]);
        return sb.ToString();
    }
}
