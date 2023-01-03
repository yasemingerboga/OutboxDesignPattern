using Confluent.Kafka;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Consumers
{
    public class PaymentResponseEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        public PaymentResponseEventConsumer()
        {
            _config = new ConsumerConfig()
            {
                BootstrapServers = "localhost:29092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                GroupId = "SM_CONSUMER",
                AllowAutoCreateTopics = false,
            };
        }

        public async Task Consume(string topic)
        {
            using var consumer = new ConsumerBuilder<string, string>(_config)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();

            consumer.Subscribe(topic);

            while (true)
            {
                try
                {
                    var consumerResult = consumer.Consume();
                    if (string.IsNullOrEmpty(consumerResult.Message.Value)) continue;

                    Console.WriteLine($"Message recieved: {consumerResult.Message.Value} - Topic: {consumerResult.Topic} - Offset: {consumerResult.Offset} - Timestamp: {consumerResult.Timestamp}");

                    var @event = JsonSerializer.Deserialize<PaymentCreatedEvent>(consumerResult.Message.Value);

                    if (@event.isPay)
                    {
                        Console.WriteLine($"Payment işleminin sonucu başarılı döndü! (payment-response-topic)");
                    }
                    else
                    {
                        Console.WriteLine($"Payment işleminin sonucu başarısız olarak döndü! (payment-response-topic)");
                    }
                    consumer.Commit(consumerResult);
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }
            }
        }
    }
}