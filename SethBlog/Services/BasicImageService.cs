using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SethBlog.Services
{
    public class BasicImageService : IImageService
    {
        public string DecodeImage(byte[] image, string contentType)
        {
            //check if this is null, if so return null
            if(image == null)
            {
                return null;
            }
            var imageString = Convert.ToBase64String(image);
            return $"data:{contentType};base64,{imageString}";
        }

        public async Task<byte[]> EncodeImageURLAsync(string imageURL)
        {
            var client = new HttpClient();

            var response = await client.GetAsync(imageURL);
            Stream stream = await response.Content.ReadAsStreamAsync();
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<byte[]> EncodeImageAsync(IFormFile image)
        {
            if(image == null)
            {
                return null;
            }
            using var ms = new MemoryStream();
            await image.CopyToAsync(ms);
            return ms.ToArray();


        }
        public Task<byte[]> EncodeImageAsync(string filename)
        {
            throw new NotImplementedException();
        }

        public string RecordContentType(IFormFile image)
        {
            if(image == null)
            {
                return null;
            }
            return image.ContentType;
        }

    }
}
