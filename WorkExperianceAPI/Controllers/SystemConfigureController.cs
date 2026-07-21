using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Work.Models.RequestModels;
using Work.Service.IService;

namespace WorkExperianceAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemConfigureController : ControllerBase
    {
        private readonly ISystemConfigureService _service;

        public SystemConfigureController(ISystemConfigureService service)
        {
            _service = service;
        }

        /// <summary>
        /// Get System Configuration
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var result = await _service.GetAsync();

            return Ok(new
            {
                Success = true,
                Message = "Data retrieved successfully.",
                Data = result
            });
        }

       
        [HttpPost("save")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Save([FromForm] SaveSystemConfigurationRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _service.SaveAsync(request);

            return Ok(new
            {
                Success = true,
                Message = "System configuration saved successfully."
            });
        }
    }
}
