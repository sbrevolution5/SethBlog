using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SethBlog.Services
{
    public class BasicFileService : IFileService
    {
        private const int DefaultMaxFileSize = (2 * 1024 * 1024); // 2 mb
        public string DecodeFile(byte[] file, string contentType)
        {
            //check if this is null, if so return null
            if(file == null || contentType == null)
            {
                return null;
            }
            var imageString = Convert.ToBase64String(file);
            return $"data:image/{contentType};base64,{imageString}";
        }

        public async Task<byte[]> EncodeFileURLAsync(string fileURL)
        {
            var client = new HttpClient();

            var response = await client.GetAsync(fileURL);
            Stream stream = await response.Content.ReadAsStreamAsync();
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> EncodeFileAsync(IFormFile file)
        {
            if(file == null)
            {
                return null;
            }
            using var ms = new MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }
        public async Task<byte[]> EncodeFileAsync(string filename)
        {
            var file = $"{Directory.GetCurrentDirectory()}/wwwroot/img/{filename}";
            return await File.ReadAllBytesAsync(file);
        }

        public string RecordContentType(IFormFile image)
        {
            if(image == null)
            {
                return null;
            }
            return image.ContentType;
        }



        public bool ValidateFileType(IFormFile file)
        {
            var authorizedTypes = new List<string> { ".jpg", ".jpeg", ".png", ".gif" };
            var validExt = authorizedTypes.Contains(RecordContentType(file));
            return validExt;
        }

        public bool ValidateFileType(IFormFile file, List<string> fileTypes)
        {
            throw new NotImplementedException();
        }

        public bool ValidateFileSize(IFormFile file)
        {
            return Size(file) < DefaultMaxFileSize;
        }

        public bool ValidateFileSize(IFormFile file, int maxSize)
        {
            return Size(file) < maxSize;
        }
        public int Size(IFormFile file)
        {
            return Convert.ToInt32(file?.Length);
        }
    }
}
