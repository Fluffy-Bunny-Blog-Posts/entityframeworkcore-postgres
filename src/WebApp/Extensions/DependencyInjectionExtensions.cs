using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.TenantAwareDbContextAccessor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WebApp.DbContextServices;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddPostgresDbContextOverrides(
            this IServiceCollection services)
        {
            services.AddSingleton<IDbContextOptionsProvider, PostgresDbContextOptionsProvider>();
            return services;
        }
        public static IServiceCollection AddInMemoryDbContextOverrides(
           this IServiceCollection services)
        {
            services.AddSingleton<IDbContextOptionsProvider, InMemoryDbContextOptionsProvider>();
            return services;
        }
        public static IServiceCollection AddDbContextTenantServices(this IServiceCollection services)
        {
            services.TryAddSingleton<ITenantAwareDbContextAccessor, TenantAwareDbContextAccessor>();
            return services;
        }
    }
    
}
