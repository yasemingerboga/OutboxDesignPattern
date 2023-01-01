using Application.Commands.Response;
using Application.Queries;
using Application.Repositories;
using DomainPayment.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Handlers
{
    public class GetOrdersByDescriptionQueryHandler : IRequestHandler<GetOrdersByDescriptionQuery, List<Order>>
    {
        private readonly IOrderRepository _orderRepository;
        public GetOrdersByDescriptionQueryHandler(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        public async Task<List<Order>> Handle(GetOrdersByDescriptionQuery request, CancellationToken cancellationToken)
        {
            var orders = await _orderRepository.GetWhere(o=>o.Description==request.Description);
            return orders;
        }


    }
}
