using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace WebApp.DbContextServices
{
    public class PostgresDbContextOptionsProvider : IDbContextOptionsProvider
    {
        private EntityFrameworkConnectionOptions _options;
        public PostgresDbContextOptionsProvider(IOptions<EntityFrameworkConnectionOptions> options)
        {
            _options = options.Value;
        }

        public void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _options.ConnectionString;
            optionsBuilder.UseNpgsql(connectionString);
            optionsBuilder.UseLazyLoadingProxies();
        }

        public void OnConfiguring(string tenantId, DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _options.ConnectionStringDatabaseTemplate
                                           .Replace("{{Database}}", $"{tenantId}-database");
            optionsBuilder.UseNpgsql(connectionString);
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
