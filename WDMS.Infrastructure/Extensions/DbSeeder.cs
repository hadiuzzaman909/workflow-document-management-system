using WDMS.Domain.Entities;
using WDMS.Domain.Enums;
using WDMS.Infrastructure.Data;
using WDMS.Infrastructure.Utils;

namespace WDMS.Infrastructure.Extensions
{
    public static class DbSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (!context.Admins.Any(a => !a.IsDeleted))
            {
                var readWriteAdmin = new Admin
                {
                    Email = "admin@wdms.com",
                    PasswordHash = PasswordHelper.CreatePasswordHash("Admin@123"), 
                    AccessLevel = AccessLevel.ReadWrite,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var readOnlyAdmin = new Admin
                {
                    Email = "viewer@wdms.com",
                    PasswordHash = PasswordHelper.CreatePasswordHash("Viewer@123"), 
                    AccessLevel = AccessLevel.ReadOnly,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Admins.AddRange(readWriteAdmin, readOnlyAdmin);
                context.SaveChanges();
            }
        }

    }
}