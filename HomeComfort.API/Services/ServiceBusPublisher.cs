using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace HomeComfort.API.Services
{
    public class ServiceBusPublisher
    {
        private readonly ServiceBusSender _sender;

        public ServiceBusPublisher(IConfiguration config)
        {
            var connectionString = config["ServiceBus:ConnectionString"];
            var queueName = config["ServiceBus:QueueName"];
            var client = new ServiceBusClient(connectionString);
            _sender = client.CreateSender(queueName);
        }

        public async Task PublishProductCreated(int productId, string productName)
        {
            var payload = new
            {
                productId,
                productName,
                eventType = "product-created",
                timestamp = DateTime.UtcNow
            };

            var messageBody = JsonSerializer.Serialize(payload);
            var message = new ServiceBusMessage(messageBody);

            await _sender.SendMessageAsync(message);
        }
    }
}
