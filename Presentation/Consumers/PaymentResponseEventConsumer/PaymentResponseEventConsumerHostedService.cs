using Application.Consumers;
using Persistance.Repositories;

namespace PaymentResponseEventConsumer
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

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Event consumer service running.");
            using (IServiceScope serviceScope = _serviceProvider.CreateScope())
            {
                var eventConsumer = serviceScope.ServiceProvider.GetRequiredService<IEventConsumer>();
                //var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                var topic = "payment-response-topic";
                await Task.Run(() => eventConsumer.ConsumeAsync(topic), cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Event consumer service stopped.");
            return Task.CompletedTask;
        }
    }
}
