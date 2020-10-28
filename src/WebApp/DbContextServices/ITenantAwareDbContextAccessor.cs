using Microsoft.EntityFrameworkCore;

namespace Microsoft.EntityFrameworkCore
{
    public interface ITenantAwareDbContextAccessor
    {
        ITenantAwareDbContext GetTenantAwareDbContext(string tenantId);
    }
}
