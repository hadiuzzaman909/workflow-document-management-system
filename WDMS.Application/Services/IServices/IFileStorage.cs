using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WDMS.Application.Services.IServices
{
    public interface IFileStorage
    {
        Task<string> SaveAsync(IFormFile file, string subFolder, CancellationToken ct = default);
        Task<(Stream Stream, string ContentType, string FileName)?> OpenReadAsync(string relativePath, CancellationToken ct = default);
        void Delete(string relativePath);
    }
}
