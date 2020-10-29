using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.TenantAwareDbContextAccessor;
using Microsoft.Extensions.DependencyInjection.Extensions;
using static Microsoft.EntityFrameworkCore.EntityFrameworkCoreDelegates;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddPostgresDbContextOverrides(
            this IServiceCollection services)
        {
            services.AddSingleton<DbContextOnConfiguringOverride>(sp => PostgresDelegates.BuildDbContextOnConfiguringOverride(sp));
            return services;
        }
        public static IServiceCollection AddInMemoryDbContextOverrides(
           this IServiceCollection services)
        {
            services.AddSingleton<DbContextOnConfiguringOverride>(sp => InMemoryDelegates.BuildDbContextOnConfiguringOverride(sp));
            return services;
        }
        public static IServiceCollection AddDbContextTenantServices(this IServiceCollection services)
        {
            services.TryAddSingleton<ITenantAwareDbContextAccessor, TenantAwareDbContextAccessor>();
            return services;
        }
    }
    
}
