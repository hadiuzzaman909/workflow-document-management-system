using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WDMS.Admin.Services;
using WDMS.Application.Services;
using WDMS.Application.Services.IServices;
using WDMS.Applocation.Services;
using WDMS.Infrastructure.Data;
using WDMS.Infrastructure.Extensions;
using WDMS.Infrastructure.Repositories;
using WDMS.Infrastructure.Repositories.IRepositories;
using WDMS.Infrastructure.Utils;

namespace WDMS.Admin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddSession(options =>
            {
                options.Cookie.Name = ".WDMS.Admin.Session";
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            builder.Services.Configure<FormOptions>(o =>
            {
                o.MultipartBodyLengthLimit = 100 * 1024 * 1024; 
            });

            builder.Services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Admin/Login";          
                    options.AccessDeniedPath = "/Admin/Login";
                    options.ExpireTimeSpan = TimeSpan.FromHours(8);
                    options.SlidingExpiration = true;
                })
                .AddJwtBearer("Jwt", options =>
                {
                    options.Authority = builder.Configuration["JwtSettings:Issuer"];
                    options.Audience = builder.Configuration["JwtSettings:Audience"];
                    options.RequireHttpsMetadata = false;
                });

            builder.Services.AddSingleton<JwtUtils>();


            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<ITaskAssignmentRepository, TaskAssignmentRepository>();
            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddScoped<IDocumentService, DocumentService>();
            builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
            builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();
            builder.Services.AddTransient<IWorkflowService, WorkflowService>();
            builder.Services.AddScoped<IWorkflowRepository, WorkflowRepository>();


            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WDMS"))
            );

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("ReadWrite", policy => policy.RequireAssertion(ctx =>
                    ctx.User.HasClaim(ClaimTypes.Role, "ReadWrite") ||
                    ctx.User.HasClaim("access_level", "ReadWrite")));
            });

            builder.Services.AddHttpContextAccessor();

            builder.Services.AddHttpClient("Api", (sp, http) =>
            {
                var cfg = sp.GetRequiredService<IConfiguration>();
                http.BaseAddress = new Uri(cfg["ApiBaseUrl"]!);
            })
            .AddHttpMessageHandler(sp =>
            {
                var accessor = sp.GetRequiredService<IHttpContextAccessor>();
                return new JwtSessionHandler(accessor);
            });

            builder.Services.AddScoped<ApiClient>();


            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }



            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();           
            app.UseAuthentication();    
            app.UseAuthorization();

            SeedDatabase(app);

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Dashboard}/{action=Index}/{id?}");

            app.Run();
        }

        private static void SeedDatabase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();
            DbSeeder.Seed(context);
        }
    }
}
