using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PaymentPersistance.Context;
using PaymentPersistance.Repositories;

namespace PaymentPersistance
{
    public static class ServiceRegistrations
    {
        public static void AddPersistenceServicesPayment(this IServiceCollection services)
        {
            services.AddDbContext<PaymentDbContext>(options => options.UseSqlServer("Server=localhost,1455;Database=PaymentDb; TrustServerCertificate=True; User = sa; Password=password123*"));
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IPaymentOutboxRepository, PaymentOutboxRepository>();
            services.AddScoped<IPaymentInboxRepository, PaymentInboxRepository>();
        }
    }
}
