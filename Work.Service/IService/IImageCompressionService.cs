using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Work.Models.EntityModels;
using Work.Models.Enums;

namespace Work.Service.IService
{
    public interface IImageCompressionService
    {
        Task<ImageResult> CompressAsync(IFormFile file, string folderName, ImageType imageType);
    }
}
