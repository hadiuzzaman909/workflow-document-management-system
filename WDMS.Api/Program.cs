using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using WDMS.Api.Extensions;
using WDMS.Application.Mappings;
using WDMS.Application.Services;
using WDMS.Application.Services.IServices;
using WDMS.Domain.Enums;
using WDMS.Infrastructure.Data;
using WDMS.Infrastructure.Repositories;
using WDMS.Infrastructure.Repositories.IRepositories;
using WDMS.Infrastructure.Services;
using WDMS.Infrastructure.Utils;

namespace WDMS.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;
            builder.Services.AddCorsPolicies(builder.Configuration);

            // Register DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WDMS"))
            );

            builder.Services.AddHttpContextAccessor();  

            builder.Services.ConfigureJwtAuth(builder.Configuration);
            builder.Services.AddCorsPolicies(builder.Configuration);
            builder.Services.AddSingleton<JwtUtils>();
            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IAdminRepository, AdminRepository>();
            builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
            builder.Services.AddScoped<ITaskAssignmentRepository, TaskAssignmentRepository>();


            // Register services
            builder.Services.AddSingleton<JwtUtils>();
            builder.Services.AddTransient<IAdminPermissionService, AdminPermissionService>();

            builder.Services.AddScoped<IAdminService, AdminService>();
            builder.Services.AddTransient<IDocumentTypeService, DocumentTypeService>();
            builder.Services.AddTransient<IWorkflowService, WorkflowService>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
            builder.Services.AddScoped<IDocumentService, DocumentService>();


            // Add Authorization Policies (Role-Based Access Control)
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

                // Permission-based policies
                options.AddPolicy("Permission.ReadOnly", policy =>
                    policy.Requirements.Add(new PermissionRequirement(AccessLevel.ReadOnly)));

                options.AddPolicy("Permission.ReadWrite", policy =>
                    policy.Requirements.Add(new PermissionRequirement(AccessLevel.ReadWrite)));
            });



            builder.Services.AddControllers();
            builder.Services.ConfigureSwagger();

            var app = builder.Build();


            // Data seeding
            //using (var scope = app.Services.CreateScope())
            //{
            //    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //    context.Database.Migrate();

            //    DbSeeder.Seed(context);
            //}

            // Middleware Pipeline
            app.UseCorsPolicies(builder.Configuration);
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerMiddleware(app.Environment);
            app.UseStaticFiles();

            app.MapControllers();
            app.Run();
        }
    }
}