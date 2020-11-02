using System;
using System.Threading.Tasks;
using WebApp.Models;

namespace Microsoft.EntityFrameworkCore
{
    public interface ITenantAwareDbContext : IDisposable
    {
        DbSet<State> States { get; set; }
        DbSet<County> Counties { get; set; }
        DbSet<City> Cities { get; set; }
        DbContext DbContext { get; }
        Task<int> SaveChangesAsync();
    }
}