using Application.Producers;
using ApplicationShipment.Reposiroty;
using DomainShipment;
using Quartz;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ShipmentProcessOutboxJob.Jobs
{
    public class ShipmentOutboxPublishJob : IJob
    {
        //readonly IPublishEndpoint _publishEndpoint;
        private readonly IEventProducer _eventProducer;
        private IShipmentOutboxRepository _shipmentOutboxRepository;
        //private IOrderOutboxRepository _orderOutboxRepository;

        public ShipmentOutboxPublishJob(IEventProducer eventProducer, IShipmentOutboxRepository shipmentOutboxRepository)
        //,IOrderOutboxRepository orderOutboxRepository)
        {
            _eventProducer = eventProducer;
            _shipmentOutboxRepository = shipmentOutboxRepository;
            //_orderOutboxRepository = orderOutboxRepository;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            var shipmentOutboxes = await _shipmentOutboxRepository.GetWhere(o => o.ProcessedDate == null);
            shipmentOutboxes.OrderByDescending(o => o.ProcessedDate);
            foreach (ShipmentOutbox shipmentOutbox in shipmentOutboxes)
            {
                if (shipmentOutbox.Type == nameof(ShipmentCreatedEvent))
                {
                    Shipment? shipment = JsonSerializer.Deserialize<Shipment>(shipmentOutbox.Payload);
                    if (shipment != null)
                    {
                        ShipmentCreatedEvent shipmentCreatedEvent = new()
                        {
                            Address = shipment.Address,
                            Id = shipment.Id,
                            OrderId = shipmentOutbox.OrderId
                        };
                        var topic = "shipment-response-topic";
                        Console.WriteLine("shipment-response-topic'e mesaj fırlatıldı!!");
                        await _eventProducer.ProduceAsync(topic, shipmentCreatedEvent);
                        shipmentOutbox.ProcessedDate = DateTime.Now;
                        await _shipmentOutboxRepository.UpdateAsync(shipmentOutbox);
                    }
                }
            }
        }
    }
}