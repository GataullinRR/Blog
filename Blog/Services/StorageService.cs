using Blog.Attributes;
using Blog.Misc;
using DBModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class StorageService
    {
        public StorageService()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns>img tag src</returns>
        public async Task<string> SaveProfileImageAsync(IFormFile formFile, User owner)
        {
            var serverLocalPath = Path.Combine("images", "users", owner.Id.ToString(), $"{Guid.NewGuid()}.{Path.GetExtension(formFile.FileName)}");
            var serverPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", serverLocalPath);
            using (var toServerStream = IOUtils.CreateFile(serverPath))
            using (var fromClientStream = formFile.OpenReadStream())
            {
                await toServerStream.WriteAsync(await fromClientStream.ReadToEndAsync());
            }

            return serverLocalPath;
        }

        public async Task<string> SavePostImageAsync(Stream file, string extension)
        {
            var serverLocalPath = Path.Combine("images", "users", "posts", $"{Guid.NewGuid()}.{extension}");
            var serverPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", serverLocalPath);
            using (var toServerStream = IOUtils.CreateFile(serverPath))
            { 
                await toServerStream.WriteAsync(await file.ReadToEndAsync());
            }

            return serverLocalPath;
        }
    }
}
