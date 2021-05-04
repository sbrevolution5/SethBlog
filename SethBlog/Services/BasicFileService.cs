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
        public string DecodeFile(byte[] file, string contentType)
        {
            //check if this is null, if so return null
            if(file == null)
            {
                return null;
            }
            var imageString = Convert.ToBase64String(file);
            return $"data:{contentType};base64,{imageString}";
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

        public string RecordContentType(IFormFile image)
        {
            if(image == null)
            {
                return null;
            }
            return image.ContentType;
        }

        public Task<byte[]> EncodeFileAsync(string filename)
        {
            throw new NotImplementedException();
        }


        public bool ValidateFileType(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public bool ValidateFileType(IFormFile file, List<string> fileTypes)
        {
            throw new NotImplementedException();
        }

        public bool ValidateFileSize(IFormFile file)
        {
            throw new NotImplementedException();
        }

        public bool ValidateFileSize(IFormFile file, int maxSize)
        {
            throw new NotImplementedException();
        }
        public int Size(IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
