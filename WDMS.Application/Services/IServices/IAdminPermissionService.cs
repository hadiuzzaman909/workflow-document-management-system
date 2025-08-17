using WDMS.Domain.Enums;

namespace WDMS.Application.Services.IServices
{
    public interface IAdminPermissionService
    {
        Task<bool> AdminHasPermissionAsync(string userId, AccessLevel requiredAccessLevel);
    }
}
