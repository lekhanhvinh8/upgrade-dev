using System.Text.Json.Serialization;
using MassTransit;
using ServiceInfoService.Events;

namespace ServiceInfoService.Consumer;

public class OrderPlacedConsumer : IConsumer<OrderPlaced>
{
    public async Task Consume(ConsumeContext<OrderPlaced> context)
    {
        //16. Connect kafka (Producer/Consumer)
        
        var order = context.Message;

        if(order.IsHealthCheck)
        {
            return;
        }

        var offset = context.Offset();
        Console.WriteLine("Consume: OffSet: " + offset.ToString() + " " + order.OrderDate.ToString());
    }
}
