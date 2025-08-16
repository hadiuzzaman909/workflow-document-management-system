using WDMS.Domain.Enums;
using System.Security.Cryptography;
using System.Text;
using WDMS.Infrastructure.Data;

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
                    PasswordHash = HashPassword("Admin@123"),
                    AccessLevel = AccessLevel.ReadWrite,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };


                var readOnlyAdmin = new Admin
                {
                    Email = "viewer@wdms.com",
                    PasswordHash = HashPassword("Viewer@123"),
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

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}