using Application.Repositories;
using DomainPayment.Entities;
using Persistance.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance.Repositories
{
    public class OrderInboxRepository : Repository<OrderInbox>, IOrderInboxRepository
    {
        public OrderInboxRepository(OrderDbContext context) : base(context)
        {

        }
    }
}
