using System.Text;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class WebhookController : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> ReceiveWebhook()
    {
        // Read the request body as string
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            var payload = await reader.ReadToEndAsync();

            // Handle webhook payload here
            // You can process the payload and perform necessary actions
            var jnode = JsonNode.Parse(payload)!.AsObject();

            // For example, log the received payload
            //Console.WriteLine("Received webhook payload: " + payload);
        }

        return Ok();
    }

    [HttpPost]
    [Route("mock")]
    public async Task<IActionResult> Mock()
    {
        // Read the request body as string
        using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
        {
            var payload = await reader.ReadToEndAsync();
            Console.WriteLine(payload);

        }

        return Ok();
    }
}