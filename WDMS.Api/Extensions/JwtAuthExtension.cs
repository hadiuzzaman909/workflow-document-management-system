using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace WDMS.Api.Extensions
{
    public static class JwtAuthExtension
    {
        public static IServiceCollection ConfigureJwtAuth(
            this IServiceCollection services,
            IConfiguration config)
        {
            var jwtSection = config.GetSection("JwtSettings");
            var secretKey = jwtSection["SecretKey"];
            var issuer = jwtSection["Issuer"];
            var audience = jwtSection["Audience"];

            services.AddAuthentication(options =>
            {
                // Use JWT defaults
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opts =>
            {
                opts.RequireHttpsMetadata = false;
                opts.SaveToken = true;
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    ClockSkew = TimeSpan.Zero
                };
            });

            return services;
        }
    }

}
