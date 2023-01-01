using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainPayment
{
    public class PaymentInbox
    {
        public int OrderId { get; set; }
        public string isPayment { get; set; }
        public bool Processed { get; set; }
        public Guid IdempotentToken { get; set; }
    }
}
