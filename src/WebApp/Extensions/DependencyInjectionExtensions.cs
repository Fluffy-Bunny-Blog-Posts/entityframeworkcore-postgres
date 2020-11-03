using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using static Microsoft.EntityFrameworkCore.TenantAwareDbContextAccessor;
using Microsoft.Extensions.DependencyInjection.Extensions;
 

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddPostgresDbContextOptionsProvider(
            this IServiceCollection services)
        {
            services.AddSingleton<IDbContextOptionsProvider, PostgresDbContextOptionsProvider>();
            return services;
        }
        public static IServiceCollection AddInMemoryDbContextOptionsProvider(
           this IServiceCollection services)
        {
            services.AddSingleton<IDbContextOptionsProvider, InMemoryDbContextOptionsProvider>();
            return services;
        }
        public static IServiceCollection AddDbContextTenantServices(this IServiceCollection services)
        {
            services.AddScoped<IAppEntityCoreContext, AppEntityCoreContext>();
            services.TryAddSingleton<ITenantAwareDbContextAccessor, TenantAwareDbContextAccessor>();
            services.AddDbContext<TenantAwareDbContext>((serviceProvider, optionsBuilder) => {
                // for NON-INMEMORY  - TenantAwareDbContext
                // this is only here so that migration models can be created.
                // we then use it as a template to not only create the new database for the tenant, but
                // downstream using it as a normal connection.
                var dbContextOptionsProvider = serviceProvider.GetRequiredService<IDbContextOptionsProvider>();
                dbContextOptionsProvider.Configure(optionsBuilder);
            });
            services.AddDbContext<AppEntityCoreContext>((serviceProvider, optionsBuilder) => {
                var dbContextOptionsProvider = serviceProvider.GetRequiredService<IDbContextOptionsProvider>();
                dbContextOptionsProvider.Configure(optionsBuilder);
            });
            return services;
        }
    }
    
}
