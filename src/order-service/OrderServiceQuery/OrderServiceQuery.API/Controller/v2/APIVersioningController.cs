using Microsoft.AspNetCore.Mvc;
using OrderServiceQuery.Core.Resources.Common;

namespace OrderServiceQuery.API.Controllers.v2
{
    [Route("api/[controller]")]
    [ApiVersion("2.0")]
    [ApiController]
    public class APIVersioningController : ControllerBase
    {
        [Route("TestAPIVer")]
        [HttpGet()]
        public async Task<IActionResult> TestApiVer2()
        {
            return Ok(new StandardResponse() {Data = "v2"});
        }
    }
}