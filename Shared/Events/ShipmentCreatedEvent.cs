using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class ShipmentCreatedEvent
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public long OrderId { get; set; }
    }
}
