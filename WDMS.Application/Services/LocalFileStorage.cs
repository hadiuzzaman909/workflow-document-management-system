using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;  
using WDMS.Application.Services.IServices;

namespace WDMS.Application.Services
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly IHostEnvironment _env; 
        private readonly string _root;

        public LocalFileStorage(IHostEnvironment env, IConfiguration cfg)
        {
            _env = env;
            _root = cfg.GetSection("Storage:Root").Value ?? "wwwroot/uploads";
        }

        public async Task<string> SaveAsync(IFormFile file, string subFolder, CancellationToken ct = default)
        {
            var rootAbs = Path.Combine(_env.ContentRootPath, _root);
            var folder = Path.Combine(rootAbs, subFolder);
            Directory.CreateDirectory(folder);

            var safeFileName = Path.GetFileName(file.FileName);
            var unique = $"{Guid.NewGuid():N}{Path.GetExtension(safeFileName)}";
            var absPath = Path.Combine(folder, unique);

            using var stream = new FileStream(absPath, FileMode.CreateNew);
            await file.CopyToAsync(stream, ct);

            var rel = Path.Combine(_root.Replace("wwwroot/", ""), subFolder, unique)
                        .Replace("\\", "/");
            return rel;
        }

        public async Task<(Stream Stream, string ContentType, string FileName)?> OpenReadAsync(string relativePath, CancellationToken ct = default)
        {
            var abs = Path.Combine(_env.ContentRootPath, "wwwroot", relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (!File.Exists(abs)) return null;
            var stream = new FileStream(abs, FileMode.Open, FileAccess.Read, FileShare.Read);
            var ext = Path.GetExtension(abs).ToLowerInvariant();
            var contentType = ext switch
            {
                ".pdf" => "application/pdf",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".gif" => "image/gif",
                _ => "application/octet-stream"
            };
            var fileName = Path.GetFileName(abs);
            return (stream, contentType, fileName);
        }

        public void Delete(string relativePath)
        {
            var abs = Path.Combine(_env.ContentRootPath, "wwwroot", relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(abs)) File.Delete(abs);
        }
    }
}
