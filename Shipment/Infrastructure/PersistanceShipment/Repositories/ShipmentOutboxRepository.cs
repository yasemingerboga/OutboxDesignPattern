using ApplicationShipment.Reposiroty;
using DomainShipment;
using PersistanceShipment.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersistanceShipment.Repositories
{
    public class ShipmentOutboxRepository : Repository<ShipmentOutbox>, IShipmentOutboxRepository
    {
        public ShipmentOutboxRepository(ShipmentDbContext context) : base(context)
        {

        }
    }
}
