using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class OrderCreatedEvent
    {
        public long OrderId { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public Guid IdempotentToken { get; set; }
        public long Step { get; set; }
    }
}
