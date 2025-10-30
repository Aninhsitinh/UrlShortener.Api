using System.Threading.Tasks;

namespace UrlShortener.Api.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(string email, string password);
        Task<AuthResponse> LoginAsync(string email, string password);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task LogoutAsync(string userId);
    }

    // Model trả về token cho client
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpireAt { get; set; }
    }
}
