using Microsoft.AspNetCore.Mvc;
using OrderServiceQuery.Core.Resources.Common;

namespace OrderServiceQuery.API.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class APIVersioningController : ControllerBase
    {
        [Route("TestAPIVer")]
        [HttpGet]
        public async Task<IActionResult> TestApiVer1()
        {
            return Ok(new StandardResponse() {Data = "v1"});
        }

       
    }
}