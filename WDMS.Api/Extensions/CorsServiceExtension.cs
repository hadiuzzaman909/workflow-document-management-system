using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WDMS.Api.Extensions
{
    public static class CorsServiceExtension
    {
        private const string SpecificPolicy = "AllowSpecificOrigins";
        private const string AllPolicy = "AllowAll";

        public static IServiceCollection AddCorsPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();

            services.AddCors(options =>
            {
                options.AddPolicy(SpecificPolicy, policy =>
                {
                    if (allowedOrigins != null && allowedOrigins.Length > 0)
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              // NOTE: AllowCredentials CANNOT be combined with AllowAnyOrigin
                              .AllowCredentials();
                    }
                    else
                    {
                        // Fallback to permissive if not configured
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                        // Do NOT call AllowCredentials() with AllowAnyOrigin()
                    }
                });

                options.AddPolicy(AllPolicy, policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            return services;
        }

        public static WebApplication UseCorsPolicies(this WebApplication app, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("AllowedOrigins").Get<string[]>();
            // Use specific policy if configured; otherwise fallback to AllowAll
            if (allowedOrigins != null && allowedOrigins.Length > 0)
                app.UseCors(SpecificPolicy);
            else
                app.UseCors(AllPolicy);

            return app;
        }
    }
}
