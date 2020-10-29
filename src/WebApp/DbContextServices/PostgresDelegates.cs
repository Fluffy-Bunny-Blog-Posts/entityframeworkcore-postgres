using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using static Microsoft.EntityFrameworkCore.EntityFrameworkCoreDelegates;


namespace Microsoft.EntityFrameworkCore
{
    public static class PostgresDelegates
    {
        public static Action<IServiceProvider, DbContextOptionsBuilder> DbContextConfigurationWithServiceProvider => (serviceProvider, optionsBuilder) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<EntityFrameworkConnectionOptions>>();
            var connectionString = options.Value.ConnectionString;
            optionsBuilder.UseNpgsql(connectionString);
            optionsBuilder.UseLazyLoadingProxies();
        };

        public static DbContextOnConfiguringOverride BuildDbContextOnConfiguringOverride(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<IOptions<EntityFrameworkConnectionOptions>>();
            var theDelegate = (DbContextOnConfiguringOverride)((tenantId, optionsBuilder) => {
                var connectionString = options
                                            .Value
                                            .ConnectionStringDatabaseTemplate
                                            .Replace("{{Database}}", $"{tenantId}-database");
                optionsBuilder.UseNpgsql(connectionString);
                optionsBuilder.UseLazyLoadingProxies();
            });
            return theDelegate; 
        }
    }
}
