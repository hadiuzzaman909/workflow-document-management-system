using WDMS.Application.DTOs;

namespace WDMS.Infrastructure.Services
{
    public interface IAdminService
    {
        Task<List<AdminResponse>> GetAllAdminsAsync();
        Task<AdminResponse?> GetAdminByIdAsync(int id);
        Task<AdminResponse> CreateAdminAsync(AdminRequest adminRequest);
        Task<bool> UpdateAdminAsync(AdminRequest adminRequest);
        Task<bool> DeleteAdminAsync(int id);
        Task<string?> GenerateJwtTokenAsync(string email, string password);
        Task<AdminResponse?> GetAdminByEmailAsync(string email);
        Task<bool> ValidateAdminCredentialsAsync(string email, string password);
    }
}