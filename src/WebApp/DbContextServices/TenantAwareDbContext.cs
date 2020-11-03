
using System.Threading.Tasks;
 
using WebApp.Models;

namespace Microsoft.EntityFrameworkCore
{
    public class TenantAwareDbContext : DbContext, ITenantAwareDbContext
    {
        private string _tenantId;
        private IDbContextOptionsProvider _dbContextOptionsProvider;


        // only used to build out migrations
        public TenantAwareDbContext(DbContextOptions<TenantAwareDbContext> options) : base(options) { }

        public TenantAwareDbContext(
            string tenantId,
            IDbContextOptionsProvider dbContextOptionsProvider)
        {
            _tenantId = tenantId;
            _dbContextOptionsProvider = dbContextOptionsProvider;
        }

        public DbContext DbContext => this;
        public DbSet<State> States { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<City> Cities { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_dbContextOptionsProvider == null || string.IsNullOrWhiteSpace(_tenantId))
            {
                base.OnConfiguring(optionsBuilder);
            }
            else
            {
                _dbContextOptionsProvider.OnConfiguring(_tenantId, optionsBuilder);
            }
        }
        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
