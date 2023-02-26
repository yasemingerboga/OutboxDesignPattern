

using Application.Consumers;
using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Persistance.Context;
using Persistance.Repositories;
using ShipmentResponseEventConsumer;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddScoped<IEventConsumer, ShipmentResponseEventConsumer.ShipmentResponseEventConsumer>();
        services.AddHostedService<ShipmentResponseEventConsumerHostedService>();
        services.AddDbContext<OrderDbContext>(options => options.UseSqlServer("Server=localhost,1444;Database=OrderDb; TrustServerCertificate=True; User = sa; Password=password123*"));
        services.AddScoped<IOrderOutboxRepository, OrderOutboxRepository>();

    })
    .Build();

await host.RunAsync();
