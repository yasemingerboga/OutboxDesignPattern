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
    public class ShipmentRepository : Repository<Shipment>, IShipmentRepository
    {
        public ShipmentRepository(ShipmentDbContext context) : base(context)
        {

        }
    }
}
