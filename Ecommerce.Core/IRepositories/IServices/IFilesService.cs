using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Core.IRepositories.IServices
{
    public interface IFilesService
    {
        Task<string> UploadFileAsync(IFormFile file , string uploadPath);
        void DeleteFile(string filePath , string folderName);
    }
}
