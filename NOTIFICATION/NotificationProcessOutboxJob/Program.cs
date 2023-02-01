using Application.Producers;
using ApplicationNotification.Repository;
using Microsoft.EntityFrameworkCore;
using NotificationProcessOutboxJob.Jobs;
using PersistanceNotification.Context;
using PersistanceNotification.Repositories;
using Quartz;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddQuartz(configurator =>
        {
            configurator.UseMicrosoftDependencyInjectionJobFactory();

            JobKey jobKey = new("NotificationOutboxPublishJob");

            configurator.AddJob<NotificationOutboxPublishJob>(options => options.WithIdentity(jobKey));

            TriggerKey triggerKey = new("NotificationOutboxPublishJob");
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
        services.AddScoped<INotificationOutboxRepository, NotificationOutboxRepository>();
        services.AddDbContext<NotificationDbContext>(options => options.UseSqlServer("Server=localhost,1466;Database=NotificationDb; TrustServerCertificate=True; User = sa; Password=password123*"));
    })
    .Build();

await host.RunAsync();