using Imagekit;
using Imagekit.Sdk;
using Microsoft.Extensions.Options;
using System.Net;

namespace Bloggie.Web.Repositories
{
    public class ImageKitImageRepository : IImageRepository
    {
        private readonly ImageKitOptions configuration;
        private readonly ImagekitClient imagekit;

        public ImageKitImageRepository(IOptions<ImageKitOptions> options)
        {
            this.configuration = options.Value;
            this.imagekit = new ImagekitClient(configuration.PublicKey,configuration.PrivateKey,configuration.UrlEndPoint);
        }

        public async Task<string> UploadAsync(IFormFile file)
        {
            var fileBytes = IFormFileToBytesArray(file);
            var fileCreatedRequest = new FileCreateRequest { file = fileBytes, fileName = Guid.NewGuid().ToString() };
            var uploadResult = await this.imagekit.UploadAsync(fileCreatedRequest);
            if (uploadResult != null && uploadResult.HttpStatusCode == (int)HttpStatusCode.OK) 
            {
                return String.Concat(configuration.UrlEndPoint, uploadResult.filePath);
            }
            return null;
        }

        public byte[] IFormFileToBytesArray(IFormFile file) 
        {
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    return fileBytes;
                }
            }
            return null;
        }
    }

    public class ImageKitOptions 
    {
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string UrlEndPoint { get; set; }
    }
}
