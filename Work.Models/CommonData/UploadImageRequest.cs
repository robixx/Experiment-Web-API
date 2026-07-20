using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Work.Models.Enums;

namespace Work.Models.CommonData
{
    public class UploadImageRequest
    {
        public IFormFile Image { get; set; } = default!;

        public ImageType ImageType { get; set; }
    }
}
