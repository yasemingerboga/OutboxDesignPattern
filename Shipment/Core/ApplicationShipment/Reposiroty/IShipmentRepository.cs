using Application.Repositories;
using DomainShipment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationShipment.Reposiroty
{
    public interface IShipmentRepository : IRepository<Shipment>
    {
    }
}