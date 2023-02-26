using Application.Consumers;
using Application.Repositories;
using Confluent.Kafka;
using Shared.Events;
using System.Text.Json;

namespace PaymentResponseEventConsumer
{
    public class PaymentResponseEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private IOrderOutboxRepository _orderOutboxRepository;
        public PaymentResponseEventConsumer(IOrderOutboxRepository orderOutboxRepository)
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

                    var @event = JsonSerializer.Deserialize<PaymentCreatedEvent>(consumerResult.Message.Value);

                    if (@event.isPay)
                    {
                        Console.WriteLine($"Payment işleminin sonucu başarılı döndü! (payment-response-topic)");
                        var orderOutbox = _orderOutboxRepository.Get(o => o.Type == nameof(PaymentCreatedEvent) && o.OrderId == @event.OrderId && o.State == 0);
                        if (orderOutbox !=null)
                        {
                            orderOutbox.State = 1;
                            orderOutbox.Payload = JsonSerializer.Serialize(@event);
                            //orderOutbox.ProcessedDate = DateTime.Now;
                            orderOutbox.Step = 1; //payment publish job içinde yapılmalı ama bağımlılık yaratır?
                            await _orderOutboxRepository.UpdateAsync(orderOutbox);
                            Console.WriteLine("OrderOutbox içerisindeki orderId = " + @event.OrderId + " olan order için PaymentCreatedEvent state'i 1 olarak işaretlendi.");
                        } else
                        {
                            Console.WriteLine("OrderOutbox kaydı bulunamadı");
                        }
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