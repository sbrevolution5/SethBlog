using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SethBlog.Services
{
    public interface IImageService
    {
        /// <summary>
        /// encode an image from an upload control
        /// </summary>
        /// <param name="image">Image from form</param>
        /// <returns>byte array of image</returns>
        Task<byte[]> EncodeImageAsync(IFormFile image);

        //encode an image from a URL
        Task<byte[]> EncodeImageURLAsync(string imageURL);
        string DecodeImage(byte[] image, string contentType);
        string RecordContentType(IFormFile image);
        

    }
}
