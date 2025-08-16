using WDMS.Application.Services.IServices;
using WDMS.Domain.Enums;
using WDMS.Infrastructure.Repositories;

namespace WDMS.Application.Services
{
    public class AdminPermissionService : IAdminPermissionService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminPermissionService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }

        public async Task<bool> AdminHasPermissionAsync(string userId, AccessLevel requiredAccessLevel)
        {
            // Fetch the admin from the repository using userId
            var admin = await _adminRepository.GetByIdAsync(int.Parse(userId)); 

            if (admin == null)
            {
                return false;
            }

            return admin.AccessLevel >= requiredAccessLevel;
        }
    }
}
