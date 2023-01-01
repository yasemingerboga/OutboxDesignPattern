using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Persistance.Context;
using Persistance.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistance
{
    public static class ServiceRegistrations
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            services.AddDbContext<OrderDbContext>(options => options.UseSqlServer("Server=localhost,1444;Database=OrderDb; TrustServerCertificate=True; User = sa; Password=password123*"));
            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderOutboxRepository, OrderOutboxRepository>();
            services.AddScoped<IOrderInboxRepository, OrderInboxRepository>();
            //services.AddScoped<IEventConsumer, PaymentResponseEventConsumer>();
            //services.AddScoped<IEventProducer,EventProducer>();
        }
    }
}
