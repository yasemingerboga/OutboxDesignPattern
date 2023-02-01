using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class NotificationCreatedEvent
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public long OrderId { get; set; }
    }
}
