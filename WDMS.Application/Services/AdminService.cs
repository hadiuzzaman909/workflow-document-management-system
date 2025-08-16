using WDMS.Application.DTOs;
using WDMS.Infrastructure.Repositories;
using System.Security.Cryptography;
using System.Text;
using WDMS.Infrastructure.Utils;


namespace WDMS.Infrastructure.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly JwtUtils _jwtUtils;

        public AdminService(IAdminRepository adminRepository, JwtUtils jwtUtils)
        {
            _adminRepository = adminRepository;
            _jwtUtils = jwtUtils;
        }

        public async Task<List<AdminResponse>> GetAllAdminsAsync()
        {
            var admins = await _adminRepository.GetActiveAdminsAsync();

            return admins.Select(a => new AdminResponse
            {
                AdminId = a.AdminId,
                Email = a.Email,
                AccessLevel = a.AccessLevel,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt,
                IsActive = a.IsActive
            }).ToList();
        }

        public async Task<AdminResponse?> GetAdminByIdAsync(int id)
        {
            var admin = await _adminRepository.GetByIdAsync(id);

            if (admin == null)
            {
                return null;
            }

            return new AdminResponse
            {
                AdminId = admin.AdminId,
                Email = admin.Email,
                AccessLevel = admin.AccessLevel,
                CreatedAt = admin.CreatedAt,
                UpdatedAt = admin.UpdatedAt,
                IsActive = admin.IsActive
            };
        }

        public async Task<AdminResponse?> GetAdminByEmailAsync(string email)
        {
            var admin = await _adminRepository.GetActiveByEmailAsync(email);

            if (admin == null)
            {
                return null;
            }

            return new AdminResponse
            {
                AdminId = admin.AdminId,
                Email = admin.Email,
                AccessLevel = admin.AccessLevel,
                CreatedAt = admin.CreatedAt,
                UpdatedAt = admin.UpdatedAt,
                IsActive = admin.IsActive
            };
        }

        public async Task<AdminResponse> CreateAdminAsync(AdminRequest adminRequest)
        {
            // Check if admin with email already exists
            var emailExists = await _adminRepository.EmailExistsAsync(adminRequest.Email);

            if (emailExists)
            {
                throw new InvalidOperationException("Admin with this email already exists.");
            }

            var admin = new Admin
            {
                Email = adminRequest.Email,
                PasswordHash = HashPassword(adminRequest.Password),
                AccessLevel = adminRequest.AccessLevel,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            var createdAdmin = await _adminRepository.AddAsync(admin);

            return new AdminResponse
            {
                AdminId = createdAdmin.AdminId,
                Email = createdAdmin.Email,
                AccessLevel = createdAdmin.AccessLevel,
                CreatedAt = createdAdmin.CreatedAt,
                UpdatedAt = createdAdmin.UpdatedAt,
                IsActive = createdAdmin.IsActive
            };
        }

        public async Task<bool> UpdateAdminAsync(AdminRequest adminRequest)
        {
            var admin = await _adminRepository.GetByIdAsync(adminRequest.AdminId);

            if (admin == null)
            {
                return false;
            }

            // Check if another admin with the same email exists (excluding current admin)
            var emailExists = await _adminRepository.EmailExistsAsync(adminRequest.Email, adminRequest.AdminId);

            if (emailExists)
            {
                throw new InvalidOperationException("Another admin with this email already exists.");
            }

            admin.Email = adminRequest.Email;
            admin.AccessLevel = adminRequest.AccessLevel;
            admin.UpdatedAt = DateTime.UtcNow;

            // Update password only if provided
            if (!string.IsNullOrEmpty(adminRequest.Password))
            {
                admin.PasswordHash = HashPassword(adminRequest.Password);
            }

            await _adminRepository.UpdateAsync(admin);
            return true;
        }

        public async Task<bool> DeleteAdminAsync(int id)
        {
            return await _adminRepository.SoftDeleteAsync(id);
        }

        public async Task<bool> ValidateAdminCredentialsAsync(string email, string password)
        {
            var admin = await _adminRepository.GetActiveByEmailAsync(email);

            if (admin == null)
            {
                return false;
            }

            return VerifyPassword(password, admin.PasswordHash);
        }

        public async Task<string?> GenerateJwtTokenAsync(string email, string password)
        {
            var admin = await _adminRepository.GetActiveByEmailAsync(email);

            if (admin == null || !VerifyPassword(password, admin.PasswordHash))
            {
                return null;
            }

            return _jwtUtils.GenerateJwtToken(admin.Email, admin.AccessLevel.ToString(), admin.AdminId);
        }

        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashedInput = HashPassword(password);
            return hashedInput == hashedPassword;
        }
    }
}