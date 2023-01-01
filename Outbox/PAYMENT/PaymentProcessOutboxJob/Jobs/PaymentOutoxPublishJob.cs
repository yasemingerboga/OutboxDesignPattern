using Application.Producers;
using Application.Repositories;
using DomainPayment;
using DomainPayment.Entities;
using Quartz;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaymentProcessOutboxJob.Jobs
{
    public class PaymentOutboxPublishJob : IJob
    {
        //readonly IPublishEndpoint _publishEndpoint;
        private readonly IEventProducer _eventProducer;
        private IPaymentOutboxRepository _paymentOutboxRepository;

        public PaymentOutboxPublishJob(IEventProducer eventProducer, IPaymentOutboxRepository paymentOutboxRepository)
        {
            _eventProducer = eventProducer;
            _paymentOutboxRepository = paymentOutboxRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var paymentOutboxes = await _paymentOutboxRepository.GetWhere(o => o.ProcessedDate == null);
            paymentOutboxes.OrderByDescending(o => o.ProcessedDate);
            foreach (PaymentOutbox paymentOutbox in paymentOutboxes)
            {
                if (paymentOutbox.Type == nameof(PaymentCreatedEvent))
                {
                    Payment? payment = JsonSerializer.Deserialize<Payment>(paymentOutbox.Payload);
                    if (payment != null)
                    {
                        PaymentCreatedEvent paymentCreatedEvent = new()
                        {
                            Id = payment.Id,
                            isPay=payment.isPay,
                            IdempotentToken = paymentOutbox.IdempotentToken
                        };

                        //var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                        var topic = "payment-response-topic";
                        await _eventProducer.ProduceAsync(topic, paymentCreatedEvent);
                    }
                }
                paymentOutbox.ProcessedDate = DateTime.Now;
                await _paymentOutboxRepository.UpdateAsync(paymentOutbox);
                Console.WriteLine("Payment outbox table checked!");
            }

        }
    }
}
