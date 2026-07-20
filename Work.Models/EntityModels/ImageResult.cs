using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work.Models.EntityModels
{
    public class ImageResult
    {
        public string FileName { get; set; } = string.Empty;

        public string RelativePath { get; set; } = string.Empty;

        public int Width { get; set; }

        public int Height { get; set; }

        public long Size { get; set; }

        public string ContentType { get; set; } = string.Empty;
    }
}
