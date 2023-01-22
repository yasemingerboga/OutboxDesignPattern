using Application.Consumers;
using ApplicationNotification.Repository;
using Confluent.Kafka;
using DomainNotification;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationNotification.Consumers
{
    public class NotificationShipmentEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private INotificationRepository _notificationRepository;
        private INotificationOutboxRepository _notificationOutboxRepository;

        //private IEventProducer _eventProducer;
        public NotificationShipmentEventConsumer( INotificationRepository notificationRepository ,INotificationOutboxRepository notificationOutboxRepository)
        //, IEventProducer eventProducer)
        {
            _config = new ConsumerConfig()
            {
                BootstrapServers = "localhost:29092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                GroupId = "SM_CONSUMER",
                AllowAutoCreateTopics = false,
            };
            _notificationOutboxRepository= notificationOutboxRepository;
            _notificationRepository = notificationRepository;
            //_eventProducer = eventProducer;
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

                    var @event = JsonSerializer.Deserialize<OrderCreatedEvent>(consumerResult.Message.Value);

                    Notification notification = new Notification() { Message = "Order notification'ı oluşturuldu." };
                    await _notificationRepository.AddAsync(notification);
                    await _notificationRepository.SaveChangesAsync();
                    Console.WriteLine("Notification Tablosuna kayıt yapıldı.");
                    NotificationOutbox notificationOutbox = new NotificationOutbox()
                    {
                        OccuredOn = DateTime.UtcNow,
                        ProcessedDate = null,
                        Payload = JsonSerializer.Serialize(notification),
                        Type = nameof(NotificationCreatedEvent),
                        IdempotentToken = Guid.NewGuid(),
                        OrderIdempotentToken = @event.IdempotentToken,
                        OrderId = @event.OrderId
                    };
                    await _notificationOutboxRepository.AddAsync(notificationOutbox);
                    await _notificationOutboxRepository.SaveChangesAsync();
                    Console.WriteLine("Notification Outbox Tablosuna kayıt yapıldı.");
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
