using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Consumers
{
    public interface IEventConsumer
    {
        Task ConsumeAsync(string topic);
    }
}
