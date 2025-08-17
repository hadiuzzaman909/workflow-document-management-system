using WDMS.Infrastructure.Repositories.IRepositories;

namespace WDMS.Infrastructure.Repositories
{
    public interface IAdminRepository : IGenericRepository<Admin>
    {
        Task<Admin?> GetByEmailAsync(string email);
        Task<Admin?> GetActiveByEmailAsync(string email);
        Task<List<Admin>> GetActiveAdminsAsync();
        Task<bool> EmailExistsAsync(string email, int? excludeAdminId = null);
        Task<bool> SoftDeleteAsync(int id);
    }
}