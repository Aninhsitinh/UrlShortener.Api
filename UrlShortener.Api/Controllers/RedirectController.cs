using Microsoft.AspNetCore.Mvc;
using UrlShortener.Api.Services;

namespace UrlShortener.Api.Controllers
{
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly IShortUrlService _shortUrlService;

        public RedirectController(IShortUrlService shortUrlService)
        {
            _shortUrlService = shortUrlService;
        }

        // GET: /r/{code}
        [HttpGet("/r/{code}")]
        public async Task<IActionResult> RedirectToOriginal(string code)
        {
            var url = await _shortUrlService.ResolveAsync(code);
            if (url == null)
                return NotFound(new { message = "Short URL not found" });

            return Redirect(url); // HTTP 302 Redirect
        }
    }
}
