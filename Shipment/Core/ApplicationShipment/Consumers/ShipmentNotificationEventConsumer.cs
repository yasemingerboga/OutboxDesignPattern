using Application.Consumers;
using ApplicationShipment.Reposiroty;
using Confluent.Kafka;
using DomainShipment;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ApplicationShipment.Consumers
{
    public class ShipmentNotificationEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private IShipmentRepository _shipmentRepository;
        private IShipmentOutboxRepository _shipmentOutboxRepository;

        //private IEventProducer _eventProducer;
        public ShipmentNotificationEventConsumer(IShipmentRepository shipmentRepository, IShipmentOutboxRepository shipmentOutboxRepository)
        //, IEventProducer eventProducer)
        {
            _config = new ConsumerConfig()
            {   
                BootstrapServers = "localhost:29092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                GroupId = "MS_CONSUMER",
                AllowAutoCreateTopics = false,
            };
            _shipmentOutboxRepository = shipmentOutboxRepository;
            _shipmentRepository = shipmentRepository;
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

                    Shipment shipment = new Shipment() { Address="Address1" };
                    await _shipmentRepository.AddAsync(shipment);
                    await _shipmentRepository.SaveChangesAsync();
                    Console.WriteLine("Shipment Tablosuna kayıt yapıldı.");
                    ShipmentOutbox shipmentOutbox = new ShipmentOutbox()
                    {
                        OccuredOn = DateTime.UtcNow,
                        ProcessedDate = null,
                        Payload = JsonSerializer.Serialize(shipment),
                        Type = nameof(ShipmentCreatedEvent),
                        IdempotentToken = Guid.NewGuid(),
                        OrderIdempotentToken = @event.IdempotentToken,
                        OrderId = @event.OrderId
                    };
                    await _shipmentOutboxRepository.AddAsync(shipmentOutbox);
                    await _shipmentOutboxRepository.SaveChangesAsync();
                    Console.WriteLine("Shipment Outbox Tablosuna kayıt yapıldı.");
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