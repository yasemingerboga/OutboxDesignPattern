using Application.Consumers;
using Persistance.Repositories;

namespace OrderAPI.Consumers
{
    public class PaymentResponseEventConsumerHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        //private OrderRepository _orderRepository;
        public PaymentResponseEventConsumerHostedService(IServiceProvider serviceProvider)
            //, OrderRepository orderRepository)
        {
            _serviceProvider = serviceProvider;
            //_orderRepository = orderRepository;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Event consumer service running.");
            using (IServiceScope serviceScope = _serviceProvider.CreateScope())
            {
                var eventConsumer = serviceScope.ServiceProvider.GetRequiredService<IEventConsumer>();
                //var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                var topic = "payment-response-topic";
                Task.Run(async () => await eventConsumer.Consume(topic), cancellationToken);
                //await _orderRepository.SaveChangesAsync();
            }
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Event consumer service stopped.");
            return Task.CompletedTask;
        }
    }
}
