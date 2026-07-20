using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Work.Models.CommonData;
using Work.Models.Enums;
using Work.Service.IService;

namespace WorkExperianceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageCompressionService _imageService;

        public ImageController(IImageCompressionService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadImageRequest request)
        {
            if (request.Image == null || request.Image.Length == 0)
                return BadRequest("Please select an image.");

            string folder = request.ImageType switch
            {
                ImageType.Profile => "Uploads/Profile",
                ImageType.Product => "Uploads/Product",
                ImageType.CompanyLogo => "Uploads/Company",
                ImageType.Banner => "Uploads/Banner",
                ImageType.Category => "Uploads/Category",
                ImageType.Gallery => "Uploads/Gallery",
                _ => "Uploads/Others"
            };

            var result = await _imageService.CompressAsync(
                request.Image,
                folder,
                request.ImageType);

            return Ok(new
            {
                Success = true,
                Message = "Image uploaded successfully.",
                Data = result
            });
        }
    }
}
