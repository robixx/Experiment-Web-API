using ImageMagick;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Work.Models.EntityModels;
using Work.Models.Enums;
using Work.Service.IService;

namespace Work.Service.Services
{
    public class ImageCompressionService : IImageCompressionService
    {
        public async Task<ImageResult> CompressAsync(IFormFile file, string folderName, ImageType imageType)
        {
            if (file == null || file.Length == 0)
                throw new Exception("Image is required.");

            var uploadFolder = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                folderName);

            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);

            using var stream = file.OpenReadStream();
            using var image = new MagickImage(stream);

            image.AutoOrient();

            if (image.Width > 12000 || image.Height > 12000)
                throw new Exception("Image resolution is too large.");

            var settings = GetSettings(image, file.Length, imageType);

            if (image.Width > settings.MaxWidth)
            {
                image.Resize((uint)settings.MaxWidth, 0);
            }

            image.ColorSpace = ColorSpace.sRGB;
            image.Quality = (uint)settings.Quality;

            image.Strip();

            var output = GetOutputFormat(imageType);

            image.Format = output.Format;

            if (output.Format == MagickFormat.Jpeg)
            {
                image.Settings.Interlace = Interlace.Plane;
            }

            var fileName = $"{Guid.NewGuid()}{output.Extension}";
            var fullPath = Path.Combine(uploadFolder, fileName);

            await image.WriteAsync(fullPath);

            var info = new FileInfo(fullPath);

            return new ImageResult
            {
                FileName = fileName,
                RelativePath = $"{folderName}/{fileName}",
                Width = (int)image.Width,
                Height = (int)image.Height,
                Size = info.Length,
                ContentType = $"image/{output.Extension.TrimStart('.')}"
            };
        }
        private (MagickFormat Format, string Extension) GetOutputFormat(ImageType imageType)
        {
            return imageType switch
            {
                ImageType.Product => (MagickFormat.WebP, ".webp"),
                ImageType.Gallery => (MagickFormat.WebP, ".webp"),
                ImageType.Category => (MagickFormat.WebP, ".webp"),
                ImageType.Banner => (MagickFormat.WebP, ".webp"),

                ImageType.CompanyLogo => (MagickFormat.Png, ".png"),

                ImageType.Profile => (MagickFormat.Jpeg, ".jpg"),

                _ => (MagickFormat.Jpeg, ".jpg")
            };
        }
        private CompressionSettings GetSettings(MagickImage image,long fileSize, ImageType imageType)
        {
            int width = (int)image.Width;

            return imageType switch
            {
                ImageType.Profile => new CompressionSettings
                {
                    MaxWidth = 400,
                    Quality = 90
                },

                ImageType.CompanyLogo => new CompressionSettings
                {
                    MaxWidth = 600,
                    Quality = 95
                },

                ImageType.Product => GetProductSettings(width, fileSize),

                ImageType.Banner => new CompressionSettings
                {
                    MaxWidth = 1920,
                    Quality = 90
                },

                ImageType.Category => new CompressionSettings
                {
                    MaxWidth = 800,
                    Quality = 90
                },

                ImageType.Gallery => new CompressionSettings
                {
                    MaxWidth = 1600,
                    Quality = 88
                },

                _ => new CompressionSettings
                {
                    MaxWidth = 1200,
                    Quality = 90
                }
            };
        }

        private CompressionSettings GetProductSettings(int width, long size)
        {
            if (width >= 8000 || size >= 15 * 1024 * 1024)
                return new CompressionSettings { MaxWidth = 2500, Quality = 82 };

            if (width >= 6000 || size >= 10 * 1024 * 1024)
                return new CompressionSettings { MaxWidth = 2200, Quality = 84 };

            if (width >= 4000 || size >= 5 * 1024 * 1024)
                return new CompressionSettings { MaxWidth = 1920, Quality = 86 };

            if (width >= 3000)
                return new CompressionSettings { MaxWidth = 1600, Quality = 88 };

            if (width >= 2000)
                return new CompressionSettings { MaxWidth = 1400, Quality = 90 };

            return new CompressionSettings
            {
                MaxWidth = width,
                Quality = 92
            };
        }
    }
}
