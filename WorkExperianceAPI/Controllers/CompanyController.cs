using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Globalization;

using WorkExperianceAPI.Models;
using WorkExperianceAPI.ModelViewer;

namespace WorkExperianceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
       
        private readonly FileSettings _fileSettings;

        public CompanyController(IOptions<FileSettings> fileSettings)
        {
           
            _fileSettings = fileSettings.Value;
        }

        [HttpPost("add-company-information")]
        public async Task<IActionResult> Create([FromForm] CompanyCreateDto dto)
        {
            return Ok();
        }

       
    }
}
