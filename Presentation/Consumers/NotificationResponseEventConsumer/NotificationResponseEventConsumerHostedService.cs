
using Application.Consumers;

namespace PaymentResponseEventConsumer
{
    public class NotificationResponseEventConsumerHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;
        public NotificationResponseEventConsumerHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Event consumer service running.");
            using (IServiceScope serviceScope = _serviceProvider.CreateScope())
            {
                var eventConsumer = serviceScope.ServiceProvider.GetRequiredService<IEventConsumer>();
                //var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC");
                var topic = "notification-response-topic";
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
