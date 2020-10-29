
using System.Threading.Tasks;
using WebApp.Models;
using static Microsoft.EntityFrameworkCore.EntityFrameworkCoreDelegates;
 

namespace Microsoft.EntityFrameworkCore
{
    public class TenantAwareDbContext : DbContext, ITenantAwareDbContext
    {
        private string _tenantId;
        private DbContextOnConfiguringOverride _onConfiguringOverride;

        // only used to build out migrations
        public TenantAwareDbContext(DbContextOptions<TenantAwareDbContext> options) : base(options) { }

        public TenantAwareDbContext(
            string tenantId,
            DbContextOnConfiguringOverride onConfiguringOverride)
        {
            _tenantId = tenantId;
            _onConfiguringOverride = onConfiguringOverride;
        }

        public DbContext DbContext => this;
        public DbSet<State> States { get; set; }
        public DbSet<County> Counties { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_onConfiguringOverride == null || string.IsNullOrWhiteSpace(_tenantId))
            {
                base.OnConfiguring(optionsBuilder);
            }
            else
            {
                _onConfiguringOverride(_tenantId, optionsBuilder);
            }
        }
        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
