using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IConfiguration _configuration;

        public EventHubController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<IActionResult> SendEvent()
        {
            var eventHubConnectionString = _configuration.GetValue<string>("EventHub:Connection");
            var eventHubName = _configuration.GetValue<string>("EventHub:ProductHubName");

            var producerClient = new EventHubProducerClient(eventHubConnectionString, eventHubName, new EventHubProducerClientOptions
            {
                RetryOptions = new EventHubsRetryOptions
                {
                    MaximumRetries = 5,
                    Delay = TimeSpan.FromSeconds(2)
                }
            });

            var eventBatch = await producerClient.CreateBatchAsync();

            var obj = new
            {
                Name = "Renicius Pagotto",
                Email = "test@test.com",
                Age = 30
            };

            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(obj))));
            await producerClient.SendAsync(eventBatch);

            return Ok($"The event has been published on {DateTime.Now}");
        }
    }
}
