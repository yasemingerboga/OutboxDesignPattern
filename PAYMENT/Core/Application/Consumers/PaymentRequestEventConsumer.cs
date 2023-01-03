using Application.Repositories;
using Confluent.Kafka;
using DomainPayment;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Consumers
{
    public class PaymentRequestEventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private IPaymentOutboxRepository _paymentOutboxRepository;
        private IPaymentRepository _paymentRepository;
        //private IEventProducer _eventProducer;
        public PaymentRequestEventConsumer(IPaymentRepository paymentRepository, IPaymentOutboxRepository paymentOutboxRepository)
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
            _paymentRepository = paymentRepository;
            _paymentOutboxRepository = paymentOutboxRepository;
            //_eventProducer = eventProducer;
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

                    Payment payment = new Payment() { Id = @event.Id, isPay = true, Name = "Credit Card" };
                    await _paymentRepository.AddAsync(payment);
                    await _paymentRepository.SaveChangesAsync();
                    Console.WriteLine("Payment Tablosuna kayıt yapıldı.");
                    PaymentOutbox paymentOutbox = new PaymentOutbox()
                    {
                        OccuredOn = DateTime.UtcNow,
                        ProcessedDate = null,
                        Payload = JsonSerializer.Serialize(payment),
                        Type = nameof(PaymentCreatedEvent),
                        IdempotentToken = Guid.NewGuid()
                    };
                    await _paymentOutboxRepository.AddAsync(paymentOutbox);
                    await _paymentOutboxRepository.SaveChangesAsync();
                    Console.WriteLine("Payment Outbox Tablosuna kayıt yapıldı.");
                    //await _eventProducer.ProduceAsync<PaymentCreatedEvent>("payment-response-topic", @event);
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
