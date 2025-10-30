using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UrlShortener.Api.Services;

namespace UrlShortener.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ShortUrlController : ControllerBase

    {
        private readonly IShortUrlService _shortUrlService;
        private readonly IConfiguration _config;

        public ShortUrlController(IShortUrlService shortUrlService, IConfiguration config)
        {
            _shortUrlService = shortUrlService;
            _config = config;
        }

        // POST: api/shorturl
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateShortUrlRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _shortUrlService.CreateAsync(request.OriginalUrl, userId);

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            return Ok(new
            {
                shortUrl = $"{baseUrl}/r/{result.Code}",
                result.OriginalUrl,
                result.CreatedAt
            });
        }

        // GET: api/shorturl
        [HttpGet]
        public async Task<IActionResult> GetMyLinks()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var links = await _shortUrlService.GetByUserAsync(userId);
            return Ok(links);
        }
    }

    public class CreateShortUrlRequest
    {
        public string OriginalUrl { get; set; } = string.Empty;
    }
}
