using Ecommerce.Core.IRepositories.IServices;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Ecommerce.Services
{
    public class FilesService : IFilesService
    {
        private readonly IConfiguration configuration;
        private readonly string baseUrl;

        public FilesService(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.baseUrl = configuration["BaseUrl"];
        }

        public async Task<string> UploadFileAsync(IFormFile file , string uploadPath)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file uploaded");

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            var relativePath = $"/files/{uniqueFileName}";
            return $"{baseUrl}{relativePath}";
        }

        public void DeleteFile(string filePath, string folderName)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("URL path cannot be null or empty");

            var fileName = Path.GetFileName(filePath);
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("Invalid file name extracted from URL path");

            var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var localFilePath = Path.Combine(uploadPath, fileName);
            if (File.Exists(localFilePath))
            {
                File.Delete(localFilePath);
            }
            else
            {
                throw new FileNotFoundException("File not found", localFilePath);
            }
        }
    }
}
