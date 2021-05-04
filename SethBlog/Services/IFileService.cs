using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog.Services
{
    public interface IFileService
    {
        /// <summary>
        /// encode an image from an upload control
        /// </summary>
        /// <param name="image">Image from form</param>
        /// <returns>byte array of image</returns>
        Task<byte[]> EncodeFileAsync(IFormFile file);
        //overload using filename from local server.
        //used for default image

        Task<byte[]> EncodeFileAsync(string filename);

        //encode an image from a URL
        Task<byte[]> EncodeFileURLAsync(string fileURL);
        bool ValidateFileType(IFormFile file);
        bool ValidateFileType(IFormFile file, List<string> fileTypes);
        bool ValidateFileSize(IFormFile file);
        bool ValidateFileSize(IFormFile file, int maxSize);
        string DecodeFile(byte[] file, string contentType);
        string RecordContentType(IFormFile file);
        int Size(IFormFile file);
        

    }
}
