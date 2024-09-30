using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MonitorService.Controllers
{
    [ApiController]
    public class HealthCheckController : ControllerBase
    {
      
        [AllowAnonymous]
        [HttpGet]
        [Route("healthCheck/check")]
        public async Task<ActionResult> Check()
        {
            ////
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("health")]
        public async Task<ActionResult> Health()
        {
            return Ok("Healthy");
        }
    }
}
