using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WDMS.Domain.Enums;

namespace WDMS.Application.Services.IServices
{
    public interface IAdminPermissionService
    {
        Task<bool> AdminHasPermissionAsync(string userId, AccessLevel requiredAccessLevel);
    }
}
