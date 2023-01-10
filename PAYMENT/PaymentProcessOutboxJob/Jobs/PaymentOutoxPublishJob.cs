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
        private IOrderOutboxRepository _orderOutboxRepository;

        public PaymentOutboxPublishJob(IEventProducer eventProducer, IPaymentOutboxRepository paymentOutboxRepository, IOrderOutboxRepository orderOutboxRepository)
        {
            _eventProducer = eventProducer;
            _paymentOutboxRepository = paymentOutboxRepository;
            _orderOutboxRepository = orderOutboxRepository;
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
                            isPay = payment.isPay,
                            IdempotentToken = paymentOutbox.IdempotentToken,
                            OrderIdempotentToken = paymentOutbox.OrderIdempotentToken
                        };

                        //var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                        var topic = "payment-response-topic";
                        await _eventProducer.ProduceAsync(topic, paymentCreatedEvent);
                    }
                }
                paymentOutbox.ProcessedDate = DateTime.Now;
                await _paymentOutboxRepository.UpdateAsync(paymentOutbox);
                var orderOutbox = await _orderOutboxRepository.GetWhere(o => o.IdempotentToken == paymentOutbox.OrderIdempotentToken);
                orderOutbox[0].Step = 2;
                await _orderOutboxRepository.UpdateAsync(orderOutbox[0]);
                Console.WriteLine("Order outbox table step = 2 is done!");
            }

        }
    }
}
