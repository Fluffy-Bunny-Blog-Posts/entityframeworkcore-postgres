using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using static Microsoft.EntityFrameworkCore.EntityFrameworkCoreDelegates;


namespace Microsoft.EntityFrameworkCore
{
    public static class InMemoryDelegates
    {
        public static Action<IServiceProvider, DbContextOptionsBuilder> DbContextConfigurationWithServiceProvider => (serviceProvider, optionsBuilder) =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<EntityFrameworkConnectionOptions>>();
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            optionsBuilder.UseLazyLoadingProxies();
        };
        public static DbContextOnConfiguringOverride BuildDbContextOnConfiguringOverride(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<IOptions<EntityFrameworkConnectionOptions>>();
            var theDelegate = (DbContextOnConfiguringOverride)((tenantId, optionsBuilder) =>
            {
                optionsBuilder.UseInMemoryDatabase($"{tenantId}-InMemoryDatabase");
                optionsBuilder.UseLazyLoadingProxies();
            });
            return theDelegate;
        }
    }
}
