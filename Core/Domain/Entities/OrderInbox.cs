using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainPayment.Entities
{
    public class OrderInbox
    {
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public bool Processed { get; set; }
        public Guid IdempotentToken { get; set; }
    }
}
