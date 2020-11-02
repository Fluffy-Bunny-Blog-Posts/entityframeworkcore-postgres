using Microsoft.EntityFrameworkCore;
using System;

namespace WebApp.DbContextServices
{
    public class InMemoryDbContextOptionsProvider : IDbContextOptionsProvider
    {
        public void Configure(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            optionsBuilder.UseLazyLoadingProxies();
        }

        public void OnConfiguring(string tenantId, DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase($"{tenantId}-InMemoryDatabase");
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
