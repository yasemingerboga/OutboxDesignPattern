using Application.Commands.Requests;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Producers
{
    public interface IEventProducer
    {
        Task ProduceAsync<T>(string topic, T @event);
        Task Produce1Async(string topic, OrderCreatedEvent orderCreatedEvent);
    }
}
