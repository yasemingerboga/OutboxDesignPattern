using Application.Repositories;
using Confluent.Kafka;
using DomainPayment.Entities;
using Microsoft.Extensions.Options;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private IOrderInboxRepository _orderInboxRepository;
        public EventConsumer(IOptions<ConsumerConfig> config, IOrderInboxRepository orderInboxRepository)
        {
            //_config = config.Value;
            _config = new ConsumerConfig()
            {
                BootstrapServers = "localhost:29092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                GroupId = "SM_CONSUMER",
                AllowAutoCreateTopics = false,
                //EnablePartitionEof = true,               
            };
            _orderInboxRepository = orderInboxRepository;
            //_orderInboxRepository = orderInboxRepository;
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
                    var @event = JsonSerializer.Deserialize<OrderCreatedEvent>(consumerResult.Message.Value);
                    if (@event != null)
                    {
                        var data = await _orderInboxRepository.GetWhere(oi => oi.IdempotentToken == @event.IdempotentToken && oi.Processed);
                        if (data.Count == 0)
                        {
                            await _orderInboxRepository.AddAsync(new()
                            {
                                Description = @event.Description,
                                IdempotentToken = @event.IdempotentToken,
                                OrderId = @event.OrderId,
                                Quantity = @event.Quantity,
                                Processed = false
                            });
                            await _orderInboxRepository.SaveChangesAsync();

                            List<OrderInbox> orderInboxes = await _orderInboxRepository.GetWhere(oi => !oi.Processed);
                            foreach (var orderInbox in orderInboxes)
                            {
                                Console.WriteLine("....Cesitli islemler uygulanıyor ve processed: true oluyor....");
                                orderInbox.Processed = true;
                            }
                            await _orderInboxRepository.SaveChangesAsync();
                        }
                    }
                    var paymentCreatedEvent = JsonSerializer.Deserialize<PaymentCreatedEvent>(consumerResult.Message.Value);
                    Console.WriteLine("....Cesitli islemler uygulandı ve payment başarılı bir şekilde gerçekleşti!!!(payment-response-topic alindi..)"); ;

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
