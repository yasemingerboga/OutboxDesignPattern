using Application.Producers;
using ApplicationShipment.Reposiroty;
using Microsoft.EntityFrameworkCore;
using PersistanceShipment.Context;
using PersistanceShipment.Repositories;
using Quartz;
using ShipmentProcessOutboxJob.Jobs;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddQuartz(configurator =>
        {
            configurator.UseMicrosoftDependencyInjectionJobFactory();

            JobKey jobKey = new("ShipmentOutboxPublishJob");

            configurator.AddJob<ShipmentOutboxPublishJob>(options => options.WithIdentity(jobKey));

            TriggerKey triggerKey = new("ShipmentOutboxPublishJob");
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
        services.AddScoped<IShipmentOutboxRepository, ShipmentOutboxRepository>();
        services.AddDbContext<ShipmentDbContext>(options => options.UseSqlServer("Server=localhost,1477;Database=ShipmentDb; TrustServerCertificate=True; User = sa; Password=password123*"));
    })
    .Build();

await host.RunAsync();