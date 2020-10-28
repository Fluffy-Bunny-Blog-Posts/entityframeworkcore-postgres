using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.TenantAwareDbContextAccessor;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDbContextPostgresTenantProvider(
            this IServiceCollection services)
        {
            services.AddSingleton<DbContextOnConfiguringOverride>(sp => {
                var options = sp.GetRequiredService<IOptions<PostgresOptions>>(); 
                var theDelegate = (DbContextOnConfiguringOverride)((tenantId, optionsBuilder) => {
                var connectionString = options
                                            .Value
                                            .ConnectionStringDatabaseTemplate
                                            .Replace("{{Database}}", $"{tenantId}-database");
                    optionsBuilder.UseNpgsql(connectionString);
                    optionsBuilder.UseLazyLoadingProxies();
                });
                return theDelegate;

            });
            services.AddSingleton<ITenantAwareDbContextAccessor, TenantAwareDbContextAccessor>();
            return services;
        }
    }
}
