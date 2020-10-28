using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
 
namespace Microsoft.EntityFrameworkCore
{
    public class TenantAwareDbContextAccessor : ITenantAwareDbContextAccessor
    {
        public delegate void DbContextOnConfiguringOverride(
               string tenantId,
               DbContextOptionsBuilder builder);

        private IServiceProvider _serviceProvider;
        private DbContextOnConfiguringOverride _dbContextOnConfiguringOverride;

        public TenantAwareDbContextAccessor(
            IServiceProvider serviceProvider,
            DbContextOnConfiguringOverride dbContextOnConfiguringOverride)
        {
            _serviceProvider = serviceProvider;
            _dbContextOnConfiguringOverride = dbContextOnConfiguringOverride;
        }
        public ITenantAwareDbContext GetTenantAwareDbContext(string tenantId)
        {
            var dbContext = new TenantAwareDbContext(tenantId, _dbContextOnConfiguringOverride);
            return dbContext;
        }
    }
}
