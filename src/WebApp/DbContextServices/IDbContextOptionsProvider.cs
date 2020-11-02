using Microsoft.EntityFrameworkCore;

namespace WebApp.DbContextServices
{
    public interface IDbContextOptionsProvider
    {
        void Configure(DbContextOptionsBuilder optionsBuilder);
        void OnConfiguring(string tenantId, DbContextOptionsBuilder optionsBuilder);
    }
}
