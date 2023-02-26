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

namespace ShipmentResponseEventConsumer
{
    public class ShipmentResponseEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private IOrderOutboxRepository _orderOutboxRepository;
        public ShipmentResponseEventConsumer(IOrderOutboxRepository orderOutboxRepository)
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

                    var @event = JsonSerializer.Deserialize<ShipmentCreatedEvent>(consumerResult.Message.Value);

                    Console.WriteLine($"Shipment işleminin sonucu döndü! (shipment-response-topic)");
                    var orderOutbox = _orderOutboxRepository.Get(o => o.Type == nameof(ShipmentCreatedEvent) && o.OrderId == @event.OrderId && o.State == 0);
                    if (orderOutbox != null)
                    {
                        orderOutbox.State = 1;
                        orderOutbox.Payload = JsonSerializer.Serialize(@event);
                        orderOutbox.ProcessedDate = DateTime.Now;
                        orderOutbox.Step = 1;
                        await _orderOutboxRepository.UpdateAsync(orderOutbox);
                    }
                    Console.WriteLine("OrderOutbox içerisindeki orderId = " + @event.OrderId + " olan order için ShipmentCreatedEvent state'i 1 (yani başarılı) olarak işaretlendi.");
                    consumer.Commit(consumerResult);
                    var isCompletedOrder = await _orderOutboxRepository.GetWhere(o => o.OrderId == @event.OrderId && o.State == 1);
                    if(isCompletedOrder.Count == 3)
                    {
                        Console.WriteLine("İşlem akışları tamamlandı! ");
                        var orderOutboxToComplete = _orderOutboxRepository.Get(o => o.Type == nameof(OrderCreatedEvent) && o.OrderId == @event.OrderId && o.State == 0);
                        if (orderOutboxToComplete != null)
                        {
                            orderOutboxToComplete.State = 1;
                            await _orderOutboxRepository.UpdateAsync(orderOutboxToComplete);
                        }
                    }
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }
            }
        }
    }
}