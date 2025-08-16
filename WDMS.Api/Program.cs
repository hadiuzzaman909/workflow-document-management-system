
using Microsoft.EntityFrameworkCore;
using WDMS.Infrastructure;
using WDMS.Infrastructure.Extensions;

namespace WDMS.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<WdmsDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WDMS")));

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Seed the database if needed
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<WdmsDbContext>();

                // Apply any pending migrations
                context.Database.Migrate();

                // Seed the database with data
                DbSeeder.Seed(context);
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();  
            app.UseAuthorization();     

            app.MapControllers();

            app.Run();
        }
    }
}
