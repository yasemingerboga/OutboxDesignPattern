using Application.Repositories;
using DomainPayment;
using Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class PaymentInboxRepository : Repository<PaymentInbox>, IPaymentInboxRepository
    {
        public PaymentInboxRepository(PaymentDbContext context) : base(context)
        {

        }
    }
}
