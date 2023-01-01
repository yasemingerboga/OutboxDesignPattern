using Application.Handlers;
using Application.Producers;
using Application.Repositories;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Persistance;
using ProcessOutboxJob.Jobs;
using Quartz;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddQuartz(configurator =>
        {
            configurator.UseMicrosoftDependencyInjectionJobFactory();

            JobKey jobKey = new("OrderOutboxPublishJob");

            configurator.AddJob<OrderOutboxPublishJob>(options => options.WithIdentity(jobKey));

            TriggerKey triggerKey = new("OrderOutboxPublishTrigger");
            configurator.AddTrigger(options => options.ForJob(jobKey)
                        .WithIdentity(triggerKey)
                        .StartAt(DateTime.UtcNow)
                        .WithSimpleSchedule
                        (
                            builder => builder.WithIntervalInSeconds(5)
                                              .RepeatForever()
                        ));
        });

        //services.AddQuartz(configurator =>
        //{
        //    configurator.UseMicrosoftDependencyInjectionJobFactory();

        //    JobKey jobKey = new("PaymentOutboxPublishJob");

        //    configurator.AddJob<OrderOutboxPublishJob>(options => options.WithIdentity(jobKey));

        //    TriggerKey triggerKey = new("PaymentOutboxPublishTrigger");
        //    configurator.AddTrigger(options => options.ForJob(jobKey)
        //                .WithIdentity(triggerKey)
        //                .StartAt(DateTime.UtcNow)
        //                .WithSimpleSchedule
        //                (
        //                    builder => builder.WithIntervalInSeconds(5)
        //                                      .RepeatForever()
        //                ));
        //});

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
        services.AddScoped<IEventProducer, EventProducer>();
        services.AddPersistenceServices();
    })
    .Build();

await host.RunAsync();