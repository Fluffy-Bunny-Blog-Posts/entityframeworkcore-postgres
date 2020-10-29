using System;


namespace Microsoft.EntityFrameworkCore
{
    public static class EntityFrameworkCoreDelegates
    {
        public delegate void DbContextOnConfiguringOverride(
             string tenantId,
             DbContextOptionsBuilder builder);
        public delegate DbContextOnConfiguringOverride DbContextOnConfiguringOverrideFunc(
            IServiceProvider serviceProvider);
    }
}
