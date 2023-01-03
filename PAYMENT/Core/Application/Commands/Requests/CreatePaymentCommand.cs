using Application.Commands.Response;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Commands.Requests
{
    public class CreatePaymentCommand : IRequest<CreatePaymentCommandResponse>
    {
        public string Name { get; set; }
        public bool isPay { get; set; }
    }
}
