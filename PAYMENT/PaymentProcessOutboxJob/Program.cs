using Application.Producers;
using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using PaymentProcessOutboxJob.Jobs;
using PaymentPersistance.Context;
using PaymentPersistance.Repositories;
using Quartz;
using Persistance.Repositories;
using Persistance.Context;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddQuartz(configurator =>
        {
            configurator.UseMicrosoftDependencyInjectionJobFactory();

            JobKey jobKey = new("PaymentOutboxPublishJob");

            configurator.AddJob<PaymentOutboxPublishJob>(options => options.WithIdentity(jobKey));

            TriggerKey triggerKey = new("PaymentOutboxPublishJob");
            configurator.AddTrigger(options => options.ForJob(jobKey)
                        .WithIdentity(triggerKey)
                        .StartAt(DateTime.UtcNow)
                        .WithSimpleSchedule
                        (
                            builder => builder.WithIntervalInSeconds(5)
                                              .RepeatForever()
                        ));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        services.AddScoped<IEventProducer, EventProducer>();
        services.AddScoped<IPaymentOutboxRepository, PaymentOutboxRepository>();
        services.AddDbContext<PaymentDbContext>(options => options.UseSqlServer("Server=localhost,1455;Database=PaymentDb; TrustServerCertificate=True; User = sa; Password=password123*"));
        services.AddDbContext<OrderDbContext>(options => options.UseSqlServer("Server=localhost,1444;Database=OrderDb; TrustServerCertificate=True; User = sa; Password=password123*"));
        services.AddScoped<IOrderOutboxRepository, OrderOutboxRepository>();
    })
    .Build();

await host.RunAsync();