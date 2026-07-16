using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Globalization;
using WorkExperianceAPI.IService;
using WorkExperianceAPI.Models;
using WorkExperianceAPI.ModelViewer;

namespace WorkExperianceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyRepository _repository;
        private readonly FileSettings _fileSettings;

        public CompanyController(ICompanyRepository repository, IOptions<FileSettings> fileSettings)
        {
            _repository = repository;
            _fileSettings = fileSettings.Value;
        }

        [HttpPost("add-company-information")]
        public async Task<IActionResult> Create([FromForm] CompanyCreateDto dto)
        {

            var companyImage = await UploadImage(dto.CompanyImage);

            var socialMedias = new List<SocialMedia>();

            for (int i = 0; i < dto.SocialMediaNames.Count; i++)
            {
                string? socialImage = null;

                if (i < dto.SocialMediaImages.Count)
                {
                    socialImage = await UploadImage(dto.SocialMediaImages[i]);
                }

                socialMedias.Add(new SocialMedia
                {
                    Id = ObjectId.GenerateNewId().ToString(),
                    Name = dto.SocialMediaNames[i],
                    Url = dto.SocialMediaUrls[i],
                    Status = dto.SocialMediaStatus[i],
                    Image = socialImage
                });
            }

            var company = new CompanyInfo
            {
                CompanyName = dto.CompanyName,
                Phone = dto.Phone,
                Email = dto.Email,
                Address = dto.Address,
                Image = companyImage,
                SocialMedias = socialMedias
            };

            await _repository.CreateAsync(company);

            return Ok(company);
        }

        private async Task<string?> UploadImage(IFormFile? file)
        {
            if (file == null || file.Length == 0)
                return null;

            if (!Directory.Exists(_fileSettings.UploadPath))
            {
                Directory.CreateDirectory(_fileSettings.UploadPath);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var fullPath = Path.Combine(_fileSettings.UploadPath, fileName);

            using var stream = new FileStream(fullPath, FileMode.Create);

            await file.CopyToAsync(stream);

            return fileName;
        }
    }
}
