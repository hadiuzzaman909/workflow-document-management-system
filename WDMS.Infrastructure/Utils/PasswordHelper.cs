using System.Security.Cryptography;
using System.Text;

namespace WDMS.Infrastructure.Utils
{
    public static class PasswordHelper
    {
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be empty or whitespace.", nameof(password));

            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (passwordHash.Length != 64)
                return false;

            if (passwordSalt.Length != 128)
                return false;

            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }
    }
}