using Application.Producers;
using ApplicationNotification.Repository;
using DomainNotification;
using Quartz;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NotificationProcessOutboxJob.Jobs
{
    public class NotificationOutboxPublishJob : IJob
    {
        //readonly IPublishEndpoint _publishEndpoint;
        private readonly IEventProducer _eventProducer;
        private INotificationOutboxRepository _notificationOutboxRepository;
        //private IOrderOutboxRepository _orderOutboxRepository;

        public NotificationOutboxPublishJob(IEventProducer eventProducer, INotificationOutboxRepository notificationOutboxRepository)
        //,IOrderOutboxRepository orderOutboxRepository)
        {
            _eventProducer = eventProducer;
            _notificationOutboxRepository = notificationOutboxRepository;
            //_orderOutboxRepository = orderOutboxRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var notificationOutboxes = await _notificationOutboxRepository.GetWhere(o => o.ProcessedDate == null);
            notificationOutboxes.OrderByDescending(o => o.ProcessedDate);
            foreach (NotificationOutbox notificationOutbox in notificationOutboxes)
            {
                if (notificationOutbox.Type == nameof(NotificationCreatedEvent))
                {
                    Notification? notification = JsonSerializer.Deserialize<Notification>(notificationOutbox.Payload);
                    if (notification != null)
                    {
                        NotificationCreatedEvent paymentCreatedEvent = new()
                        {
                            Message=notification.Message,
                            Id=notification.Id,
                            OrderId = notificationOutbox.OrderId
                        };
                        var topic = "notification-response-topic";
                        Console.WriteLine("notification-response-topic'e mesaj fırlatıldı!!");
                        await _eventProducer.ProduceAsync(topic, paymentCreatedEvent);
                        notificationOutbox.ProcessedDate = DateTime.Now;
                        await _notificationOutboxRepository.UpdateAsync(notificationOutbox);
                    }
                }
            }
        }
    }
}
