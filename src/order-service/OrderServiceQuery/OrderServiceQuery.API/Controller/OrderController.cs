using MassTransit;
using Microsoft.AspNetCore.Mvc;
using OrderServiceQuery.API.Events;
using OrderServiceQuery.Core.Commands;
using OrderServiceQuery.Core.Event;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Core.Resources.CreateOrder;

namespace OrderServiceQuery.API.Controllers.OrderController
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly ICommandDispatcher _commandDispatcher;
        public OrderController(ICommandDispatcher commandDispatcher)
        {
            _commandDispatcher = commandDispatcher;
        }

        [HttpPost]
        [Route("CreateOrder")]
        public async Task<IActionResult> CreateOrder(CreateOrderRequest request)
        {
            var result = await _commandDispatcher.Dispatch<CreateOrderCommand, int>(new CreateOrderCommand(request), CancellationToken.None);
            return Ok(result);
        }

        [HttpPost]
        [Route("UpdateOrder")]
        public async Task<IActionResult> UpdateOrder(UpdateOrderRequest request)
        {
            var result = await _commandDispatcher.Dispatch<UpdateOrderCommand, int>(new UpdateOrderCommand(request), CancellationToken.None);
            return Ok(result);
        }

        
    }
}
