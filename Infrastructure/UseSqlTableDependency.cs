using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.Data;
using Microsoft.AspNetCore.Builder;

namespace AvibaWeb.Infrastructure
{
    public static class UseSqlTableDependencyHelpers
    {
        public static void UseSqlTableDependency(this IApplicationBuilder services, string connectionString)
        {
            var serviceProvider = services.ApplicationServices;
            var subscription = (IDatabaseSubscription)serviceProvider.GetService(typeof(IDatabaseSubscription));
            subscription.Configure(connectionString);
        }
    }
}
