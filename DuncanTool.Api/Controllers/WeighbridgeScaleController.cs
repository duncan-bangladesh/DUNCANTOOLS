using DuncanTool.Api.Model;
using DuncanTool.Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace DuncanTool.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeighbridgeScaleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private WeighbridgeScaleDataRepository weighbridgeScaleDataRepository;
        public WeighbridgeScaleController(IConfiguration configuration)
        {
            _configuration = configuration;
            weighbridgeScaleDataRepository = new WeighbridgeScaleDataRepository(_configuration);
        }
        [HttpPost("SaveScaleData")]
        public async Task<IActionResult> SaveScaleData(WeighbridgeScaleData model)
        {
            try
            {
                int result = await weighbridgeScaleDataRepository.SaveScaleData(model);
                if (result > 0)
                {
                    return Ok(new { Message = "success" });
                }
                else
                {
                    return BadRequest(new { Message = "failed" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "error", Error = ex.Message });
            }

        }
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { Message = "API GET request is working!" });
        }
    }
}
