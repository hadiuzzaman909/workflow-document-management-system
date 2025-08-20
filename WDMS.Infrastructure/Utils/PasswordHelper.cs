using System.Security.Cryptography;
using System.Text;

namespace WDMS.Infrastructure.Utils
{
    public static class PasswordHelper
    {
        public static string CreatePasswordHash(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty or whitespace.", nameof(password));

            using var sha512 = SHA512.Create();
            var passwordHash = sha512.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(passwordHash);
        }

        public static bool VerifyPasswordHash(string password, string storedPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrEmpty(storedPasswordHash))
                return false;

            var hashedPassword = CreatePasswordHash(password);

            return hashedPassword == storedPasswordHash; 
        }
    }
}
