using Application.Commands.Requests;
using Application.Commands.Response;
using Application.Repositories;
using DomainPayment;
using MediatR;
using System.Text.Json;

namespace Application.Handlers
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, CreatePaymentCommandResponse>
    {

        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentOutboxRepository _paymentOutboxRepository;
        public CreatePaymentCommandHandler(IPaymentRepository paymentRepository, IPaymentOutboxRepository paymentOutboxRepository)
        {
            _paymentRepository = paymentRepository;
            _paymentOutboxRepository = paymentOutboxRepository;
        }
        public async Task<CreatePaymentCommandResponse> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            Payment newPayment = new Payment { Name = request.Name, isPay = request.isPay };
            await _paymentRepository.AddAsync(newPayment);
            //burda olmalı mı olmamalı mı? (id getirmek için eklendi.)
            await _paymentRepository.SaveChangesAsync();
            PaymentOutbox orderOutbox = new()
            {
                OccuredOn = DateTime.UtcNow,
                ProcessedDate = null,
                Payload = JsonSerializer.Serialize(newPayment),
                Type = nameof(CreatePaymentCommand),
                IdempotentToken = Guid.NewGuid()
            };
            await _paymentOutboxRepository.AddAsync(orderOutbox);
            await _paymentOutboxRepository.SaveChangesAsync();
            return new CreatePaymentCommandResponse { Id = newPayment.Id, isPay = newPayment.isPay, Name = newPayment.Name };
        }
    }
}
