using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Shared.Events;
using System.Text.Json;

namespace StockAPI.Consumers
{
    //kullanılmıyor
    public class OrderCreatedEventConsumer : IHostedService
    {
        private readonly ConsumerConfig _config;
        //private IOrderInboxRepository _orderInboxRepository;

        public OrderCreatedEventConsumer(IOptions<ConsumerConfig> config)
        {
            //_config = config.Value;
            //var consumergroup = Environment.GetEnvironmentVariable("CONSUMER_GROUP");
            _config = new ConsumerConfig()
            {
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                GroupId = "SM_CONSUMER",
                AllowAutoCreateTopics = true,
                //EnablePartitionEof = true,
            };
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Consume edilmeye baslandi.");

            using var consumer = new ConsumerBuilder<string, string>(_config)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();
            var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
            consumer.Subscribe(topic);

            while (true)
            {
                try
                {
                    Console.WriteLine($"okuyor");

                    var consumerResult = consumer.Consume(cancellationToken);
                    //if (consumerResult == null)
                    //{
                    //    continue;
                    //}
                    Console.WriteLine($"{DateTime.Today} Recieved: {consumerResult.Offset}");

                    Console.WriteLine("consume geçti");

                    if (string.IsNullOrEmpty(consumerResult.Message.Value)) continue;

                    Console.WriteLine($"Message: {consumerResult.Message.Value} - Topic: {consumerResult.Topic} - Offset: {consumerResult.Offset} - Timestamp: {consumerResult.Timestamp}");

                    var @event = JsonSerializer.Deserialize<OrderCreatedEvent>(consumerResult.Message.Value);


                    //var data = await _orderInboxRepository.GetWhere(oi => oi.IdempotentToken == @event.IdempotentToken && oi.Processed);
                    //bool hasData = data.Any();

                    //if (!hasData)
                    //{
                    //    await _orderInboxRepository.AddAsync(new()
                    //    {
                    //        Description = @event.Description,
                    //        IdempotentToken = @event.IdempotentToken,
                    //        OrderId = @event.OrderId,
                    //        Quantity = @event.Quantity,
                    //        Processed = false
                    //    });
                    //    await _orderInboxRepository.SaveChangesAsync();

                    //    List<OrderInbox> orderInboxes = await _orderInboxRepository.GetWhere(oi => !oi.Processed);
                    //    foreach (var orderInbox in orderInboxes)
                    //    {
                    //        Console.WriteLine(@$"OrderId : {orderInbox.OrderId}
                    //                     Id : {orderInbox.IdempotentToken}
                    //                     Stock {orderInbox.Quantity} miktar kadar düşürülmüştür!");
                    //        orderInbox.Processed = true;
                    //    }
                    //    await _orderInboxRepository.SaveChangesAsync();
                    //}

                    consumer.Commit(consumerResult);
                }
                catch (ConsumeException e)
                {
                    Console.WriteLine($"Error occured: {e.Error.Reason}");
                }
            }
            consumer.Close();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
