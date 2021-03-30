using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace EventHubConnection.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EventHubController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            const string eventHubConnectionString = "<your event hub connection string>";
            const string eventHubName = "<your event hub name>";

            await using (var producerClient = new EventHubProducerClient(eventHubConnectionString, eventHubName))
            {
                // Create a batch of events 
                using EventDataBatch eventBatch = await producerClient.CreateBatchAsync();

                var obj = new
                {
                    Product = "Chocolate",
                    Price = 10
                };

                // Add events to the batch. An event is a represented by a collection of bytes and metadata. 
                eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj))));

                // Use the producer client to send the batch of events to the event hub
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine("A batch of 3 events has been published.");
            }

            return Ok();
        }
    }
}
