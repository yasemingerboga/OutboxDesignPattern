using Application.Repositories;
using DomainPayment;
using PaymentPersistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentPersistance.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(PaymentDbContext context) : base(context)
        {

        }
    }
}
