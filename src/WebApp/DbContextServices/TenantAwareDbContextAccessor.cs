using Microsoft.EntityFrameworkCore;
using System;
using WebApp.DbContextServices;

namespace Microsoft.EntityFrameworkCore
{
    public class TenantAwareDbContextAccessor : ITenantAwareDbContextAccessor
    {
      
        private IServiceProvider _serviceProvider;
        private IDbContextOptionsProvider _dbContextOptionsProvider;

        public TenantAwareDbContextAccessor(
            IServiceProvider serviceProvider,
            IDbContextOptionsProvider dbContextOptionsProvider)
        {
            _serviceProvider = serviceProvider;
            _dbContextOptionsProvider = dbContextOptionsProvider;
        }
        public ITenantAwareDbContext GetTenantAwareDbContext(string tenantId)
        {
            var dbContext = new TenantAwareDbContext(tenantId, _dbContextOptionsProvider);
            return dbContext;
        }
    }
}
