using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Events
{
    public class PaymentCreatedEvent
    {
        public int Id { get; set; }
        public bool isPay { get; set; }
        public Guid IdempotentToken { get; set; }
    }
}
