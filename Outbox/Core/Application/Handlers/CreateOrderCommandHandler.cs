using Application.Commands.Requests;
using Application.Commands.Response;
using Application.Repositories;
using DomainPayment.Entities;
using MediatR;
using Shared.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.Handlers
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderCommandResponse>
    {

        private readonly IOrderRepository _orderRepository;
        private readonly IOrderOutboxRepository _orderOutboxRepository;
        public CreateOrderCommandHandler(IOrderRepository orderRepository, IOrderOutboxRepository orderOutboxRepository)
        {
            _orderRepository = orderRepository;
            _orderOutboxRepository = orderOutboxRepository;
        }
        public async Task<CreateOrderCommandResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            Order newOrder = new Order { Description = request.Description, Quantity = request.Quantity, CreatedAt = DateTime.Today };
            await _orderRepository.AddAsync(newOrder);
            await _orderRepository.SaveChangesAsync();
            Console.WriteLine($"Order tablosuna kayıt yapıldı.");
            OrderOutbox orderOutbox = new()
            {
                OccuredOn = DateTime.UtcNow,
                ProcessedDate = null,
                Payload = JsonSerializer.Serialize(newOrder),
                Type = nameof(OrderCreatedEvent),
                IdempotentToken = Guid.NewGuid()
            };
            await _orderOutboxRepository.AddAsync(orderOutbox);
            await _orderOutboxRepository.SaveChangesAsync();
            Console.WriteLine($"Order Outbox tablosuna kayıt yapıldı.");
            return new CreateOrderCommandResponse { Description = newOrder.Description, CreatedAt = newOrder.CreatedAt, Quantity = newOrder.Quantity, Id = newOrder.Id };
        }
    }
}
