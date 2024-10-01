using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Core.Resources.Common;
using OrderServiceQuery.Infrastructure.CommonHelper;
using OrderServiceQuery.Infrastructure.UnitOfWork;

namespace OrderServiceQuery.API.Controllers.OrderController
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [ApiVersion("2.0")]
    public class OrderController : ControllerBase
    {
        private readonly IAppUnitOfWork<ReadSide> _repository;
        private readonly IAppUnitOfWork<WriteSide> _unitOfWork;
        public OrderController(IAppUnitOfWork<ReadSide> repository, IAppUnitOfWork<WriteSide> unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Route("GetOrder")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var result = await _repository.Orders.GetOrderByOrderIdAsync(orderId);
            return Ok(result);
        }

        [HttpGet]
        [Route("AuthenWithLastSchema")]
        [Authorize()]
        public async Task<IActionResult> AuthenWithLastSchema()
        {
            return Ok("Ok");
        }

        [HttpGet]
        [Route("AuthenWithAPIKey1")]
        [Authorize(AuthenticationSchemes = "ApiKey1")]
        public async Task<IActionResult> AuthenWithAPIKey1()
        {
            return Ok("Ok");
        }

        [HttpGet]
        [Route("AuthenWithAPIKey2")]
        [Authorize(AuthenticationSchemes = "ApiKey2")]
        public async Task<IActionResult> AuthenWithAPIKey2()
        {
            return Ok("Ok");
        }

        [HttpGet]
        [Route("AuthenWithOrSchema")]
        [Authorize(AuthenticationSchemes = "ApiKey1, ApiKey2")]
        public async Task<IActionResult> AuthenWithOrSchema()
        {
            return Ok("Ok");
        }

        [HttpGet]
        [Route("AuthenWithJwtBearer")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AuthenWithJwtBearer()
        {
            return Ok("Ok");
        }

        [HttpGet]
        [Route("TestGlobalExceptionHandle")]
        public async Task<IActionResult> TestGlobalExceptionHandle()
        {
            throw new Exception("Exception");
        }

        [HttpGet]
        [Route("TestFileHandler")]
        public async Task<IActionResult> TestFileHandler()
        {
            
            var obj = CommonHelper.ReadDataByKey<TestObject>("C:/projects/upgrade-dev-ftel-project/src/order-service/OrderServiceQuery/OrderServiceQuery.Infrastructure/Helper/test.json", "testobject");
            var excelRows = CommonHelper.ReadExcelData<TestObject>("C:/projects/upgrade-dev-ftel-project/src/order-service/OrderServiceQuery/OrderServiceQuery.Infrastructure/Helper/mysheet.xlsx");

            return Ok(new StandardResponse() {Data = new {jsonObject = obj, ExcelRows = excelRows, now = CommonHelper.ToIso8601(DateTime.Now)}});
        }

       

        private class TestObject
        {
            public string Prop1 { get; set;}
            public string Prop2 { get; set;}

        };
    }
}
