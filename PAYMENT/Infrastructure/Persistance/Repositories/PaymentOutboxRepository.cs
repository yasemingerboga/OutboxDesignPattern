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
    public class PaymentOutboxRepository : Repository<PaymentOutbox>, IPaymentOutboxRepository
    {
        public PaymentOutboxRepository(PaymentDbContext context) : base(context)
        {

        }
    }
}
