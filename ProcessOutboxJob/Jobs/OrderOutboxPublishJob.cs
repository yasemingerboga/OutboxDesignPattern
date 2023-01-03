using Application.Producers;
using Application.Repositories;
using Confluent.Kafka;
using DomainPayment.Entities;
using ProcessOutboxJob.Contexts;
using Quartz;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProcessOutboxJob.Jobs
{
    public class OrderOutboxPublishJob : IJob
    {
        //readonly IPublishEndpoint _publishEndpoint;
        private readonly IEventProducer _eventProducer;
        private IOrderOutboxRepository _orderOutboxRepository;
        private IOrderRepository _orderRepository;


        public OrderOutboxPublishJob(IEventProducer eventProducer, IOrderOutboxRepository orderOutboxRepository,
            IOrderRepository orderRepository)
        {
            _eventProducer = eventProducer;
            _orderOutboxRepository = orderOutboxRepository;
            _orderRepository = orderRepository;
            //_orderOutboxRepository = orderOutboxRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var orderOutboxes = await _orderOutboxRepository.GetWhere(o => o.ProcessedDate == null);
            orderOutboxes.OrderByDescending(o => o.ProcessedDate);
            foreach (OrderOutbox orderOutbox in orderOutboxes)
            {
                if (orderOutbox.Type == nameof(OrderCreatedEvent))
                {
                    Order? order = JsonSerializer.Deserialize<Order>(orderOutbox.Payload);
                    if (order != null)
                    {
                        OrderCreatedEvent orderCreatedEvent = new()
                        {
                            Description = order.Description,
                            OrderId = order.Id,
                            Quantity = order.Quantity,
                            IdempotentToken = orderOutbox.IdempotentToken
                        };

                        //var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                        var topic = "payment-request-topic";
                        await _eventProducer.Produce1Async(topic, orderCreatedEvent);
                        //wait for payment-response-topic;
                        //başarılıysa order add gerçekleşecek
                        //değilse gerçekleşmeyecek?
                    }
                }
                orderOutbox.ProcessedDate = DateTime.Now;
                await _orderOutboxRepository.UpdateAsync(orderOutbox);
                Console.WriteLine("Order outbox table checked!");
            }

        }
    }
}
