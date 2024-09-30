using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderServiceQuery.API.Events;
using OrderServiceQuery.Core.Commands;
using OrderServiceQuery.Core.Event;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Core.Resources.CreateOrder;
using OrderServiceQuery.Infrastructure.UnitOfWork;

namespace OrderServiceQuery.API.Controllers.OrderController
{
    [Route("api/[controller]")]
    [ApiController]
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
    }
}
