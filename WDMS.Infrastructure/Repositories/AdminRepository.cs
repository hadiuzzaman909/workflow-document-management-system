using Microsoft.EntityFrameworkCore;
using WDMS.Domain.Entities;
using WDMS.Infrastructure.Data;

namespace WDMS.Infrastructure.Repositories
{
    public class AdminRepository : GenericRepository<Admin>, IAdminRepository
    {
        public AdminRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Admin?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Admin?> GetActiveByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.Email == email &&
                                                       !a.IsDeleted &&
                                                       a.IsActive);
        }

        public async Task<List<Admin>> GetActiveAdminsAsync()
        {
            return await _dbSet.Where(a => !a.IsDeleted && a.IsActive)
                              .ToListAsync();
        }

        public async Task<bool> EmailExistsAsync(string email, int? excludeAdminId = null)
        {
            var query = _dbSet.Where(a => a.Email == email && !a.IsDeleted);

            if (excludeAdminId.HasValue)
            {
                query = query.Where(a => a.AdminId != excludeAdminId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var admin = await _dbSet.FirstOrDefaultAsync(a => a.AdminId == id && !a.IsDeleted);

            if (admin == null)
            {
                return false;
            }

            admin.IsDeleted = true;
            admin.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public override async Task<List<Admin>> GetAllAsync()
        {
            return await _dbSet.Where(a => !a.IsDeleted).ToListAsync();
        }

        public override async Task<Admin?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(a => a.AdminId == id && !a.IsDeleted);
        }
    }
}