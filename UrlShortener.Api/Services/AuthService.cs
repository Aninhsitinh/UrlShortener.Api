using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _config;
        private readonly ApplicationDbContext _db;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration config,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _db = db;
        }

        // 🟩 Đăng ký người dùng mới
        public async Task<AuthResponse> RegisterAsync(string email, string password)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
                throw new Exception("Email already registered.");

            var user = new ApplicationUser { UserName = email, Email = email };
            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            return await GenerateTokensAsync(user);

        }

        // 🟦 Đăng nhập
        public async Task<AuthResponse> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                throw new Exception("Invalid email or password.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
                throw new Exception("Invalid email or password.");

            return await GenerateTokensAsync(user);
        }

        // 🟨 Làm mới token
        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            var stored = _db.RefreshTokens.FirstOrDefault(r => r.Token == refreshToken && !r.IsRevoked);
            if (stored == null || stored.ExpiresAt < DateTime.UtcNow)
                throw new Exception("Invalid or expired refresh token.");

            var user = await _userManager.FindByIdAsync(stored.UserId);
            if (user == null)
                throw new Exception("User not found.");

            stored.IsRevoked = true;
            await _db.SaveChangesAsync();

            return await GenerateTokensAsync(user);
        }

        // 🟥 Đăng xuất
        public async Task LogoutAsync(string userId)
        {
            var tokens = _db.RefreshTokens.Where(r => r.UserId == userId && !r.IsRevoked);
            foreach (var t in tokens)
                t.IsRevoked = true;

            await _db.SaveChangesAsync();
        }

        // 🔐 Sinh Access Token + Refresh Token
        private async Task<AuthResponse> GenerateTokensAsync(ApplicationUser user)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSection["AccessTokenExpirationMinutes"])),
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = new RefreshToken
            {
                Token = GenerateRefreshToken(),
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSection["RefreshTokenExpirationDays"])),
                IsRevoked = false
            };

            _db.RefreshTokens.Add(refreshToken);
            await _db.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpireAt = token.ValidTo
            };
        }

        private string GenerateRefreshToken()
        {
            var bytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes);
        }
    }
}
