using Application.Consumers;
using Application.Repositories;
using Confluent.Kafka;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NotificationResponseEventConsumer
{
    public class NotificationResponseEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private IOrderOutboxRepository _orderOutboxRepository;
        public NotificationResponseEventConsumer(IOrderOutboxRepository orderOutboxRepository)
        {
            _config = new ConsumerConfig()
            {
                BootstrapServers = "localhost:29092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                GroupId = "SM_CONSUMER",
                AllowAutoCreateTopics = false,
            };
            _orderOutboxRepository = orderOutboxRepository;
        }

        public async Task ConsumeAsync(string topic)
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

                    var @event = JsonSerializer.Deserialize<NotificationCreatedEvent>(consumerResult.Message.Value);

                    Console.WriteLine($"Notification işleminin sonucu döndü! (notification-response-topic)");
                    var orderOutbox = _orderOutboxRepository.Get(o => o.Type == nameof(NotificationCreatedEvent) && o.OrderId == @event.OrderId && o.State == 0);
                    if (orderOutbox != null)
                    {
                        orderOutbox.State = 1;
                        orderOutbox.Payload = JsonSerializer.Serialize(@event);
                        orderOutbox.ProcessedDate = DateTime.Now;
                        orderOutbox.Step = 1;
                        await _orderOutboxRepository.UpdateAsync(orderOutbox);
                    }
                    Console.WriteLine("OrderOutbox içerisindeki orderId = " + @event.OrderId + " olan order için NotificationCreatedEvent state'i 1 olarak işaretlendi.");
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