using Microsoft.EntityFrameworkCore;
using System;
using static Microsoft.EntityFrameworkCore.EntityFrameworkCoreDelegates;


namespace Microsoft.EntityFrameworkCore
{
    public class TenantAwareDbContextAccessor : ITenantAwareDbContextAccessor
    {
      
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
