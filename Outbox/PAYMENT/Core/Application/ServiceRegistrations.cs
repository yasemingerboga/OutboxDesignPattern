using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public static class ServiceRegistrations
    {
        public static void AddApplicationServices (this IServiceCollection services)
        {
            //services.AddMediatR(typeof(ServiceRegistrations));
        }
    }
}
