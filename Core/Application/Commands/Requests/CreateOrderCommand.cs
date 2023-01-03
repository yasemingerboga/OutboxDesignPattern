using Application.Commands.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Requests
{
    public class CreateOrderCommand : IRequest<CreateOrderCommandResponse>
    {
        public string Description { get; set; }
        public int Quantity { get; set; }
    }
}
