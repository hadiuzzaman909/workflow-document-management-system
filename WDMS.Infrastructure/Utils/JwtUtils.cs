using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WDMS.Infrastructure.Utils
{
    public class JwtUtils
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expirationHours;

        public JwtUtils(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));

            var secretKey = _config["JwtSettings:SecretKey"] ??
                throw new ArgumentNullException("JwtSettings:SecretKey is missing in configuration.");

            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            _issuer = _config["JwtSettings:Issuer"] ??
                throw new ArgumentNullException("JwtSettings:Issuer is missing in configuration.");

            _audience = _config["JwtSettings:Audience"] ??
                throw new ArgumentNullException("JwtSettings:Audience is missing in configuration.");

            _expirationHours = int.Parse(_config["JwtSettings:ExpirationHours"] ?? "24");
        }

        public string GenerateJwtToken(string email, string accessLevel, int adminId)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, email),
        new Claim(ClaimTypes.Role, "Admin"), // Always Admin for the role
        new Claim("AccessLevel", accessLevel), // ReadOnly or ReadWrite
        new Claim("AdminId", adminId.ToString()),
        new Claim(JwtRegisteredClaimNames.Sub, email),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(JwtRegisteredClaimNames.Iat, new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
    };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_expirationHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = true,
                    ValidIssuer = _issuer,
                    ValidateAudience = true,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}