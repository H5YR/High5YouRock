using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using H5YR.Core.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace H5YR.Core.Services
{
    public interface IWidgetJwtService
    {
        string GenerateToken(string authProvider, string externalUserId, string displayName,
            string avatarUrl, string profileUrl);

        ClaimsPrincipal? ValidateToken(string token);
    }

    public class WidgetJwtService : IWidgetJwtService
    {
        private readonly WidgetSettings _settings;

        public WidgetJwtService(IOptions<WidgetSettings> settings)
        {
            _settings = settings.Value;
        }

        public string GenerateToken(string authProvider, string externalUserId, string displayName,
            string avatarUrl, string profileUrl)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSigningKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("auth_provider", authProvider),
                new Claim("external_user_id", externalUserId),
                new Claim("display_name", displayName),
                new Claim("avatar_url", avatarUrl),
                new Claim("profile_url", profileUrl)
            };

            var token = new JwtSecurityToken(
                issuer: "h5yr.com",
                audience: "h5yr-widget",
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.JwtSigningKey!));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "h5yr.com",
                ValidateAudience = true,
                ValidAudience = "h5yr-widget",
                ValidateLifetime = true,
                IssuerSigningKey = key,
                ClockSkew = TimeSpan.FromMinutes(5)
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                return handler.ValidateToken(token, validationParameters, out _);
            }
            catch
            {
                return null;
            }
        }
    }
}
