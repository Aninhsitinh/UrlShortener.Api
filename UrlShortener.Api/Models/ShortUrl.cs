namespace UrlShortener.Api.Models
{
    public class ShortUrl
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string OriginalUrl { get; set; } = null!;
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public int HitCount { get; set; } = 0;
    }
}
