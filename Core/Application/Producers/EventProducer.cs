using Application.Commands.Requests;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Producers
{
    public class EventProducer : IEventProducer
    {
        private readonly ProducerConfig _config;
        //ILogger<EventProducer> _logger;
        public EventProducer(IOptions<ProducerConfig> config)
        {
            //_config = config.Value;
            _config = new ProducerConfig()
            {
                BootstrapServers = "localhost:29092",
                ClientId = "SM_CONSUMER",
            };
        }

        public async Task ProduceAsync<T>(string topic, T @event)
        {
            using var producer = new ProducerBuilder<string, string>(_config)
               .SetKeySerializer(Serializers.Utf8)
               .SetValueSerializer(Serializers.Utf8)
               .Build();
            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(@event)
            };
            //_logger.LogInformation($"Produce created message event: {eventMessage.Value}");
            Console.WriteLine($"Produce created message event: {eventMessage.Value}");
            var deliveryResult = await producer.ProduceAsync(topic, eventMessage);
            Console.WriteLine($"{DateTime.Today} Delivered to {deliveryResult.Offset}");
            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception($"Could not produce {@event.GetType().Name} message to topic - {topic} due to the following reason:{deliveryResult.Message}");
            }
            producer.Flush();
        }


        public async Task Produce1Async(string topic, OrderCreatedEvent orderCreatedEvent)
        {
            using var producer = new ProducerBuilder<string, string>(_config)
                .SetKeySerializer(Serializers.Utf8)
                .SetValueSerializer(Serializers.Utf8)
                .Build();
            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(orderCreatedEvent)
            };
            //_logger.LogInformation($"Produce created message event: {eventMessage.Value}");
            Console.WriteLine($"Produce created message event: {eventMessage.Value}");
            var deliveryResult = await producer.ProduceAsync(topic, eventMessage);
            Console.WriteLine($"{DateTime.Today} Delivered to {deliveryResult.Offset}");
            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception($"Could not produce {orderCreatedEvent.GetType().Name} message to topic - {topic} due to the following reason:{deliveryResult.Message}");
            }
            producer.Flush();
        }
    }
}