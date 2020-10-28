using System;
using System.Threading.Tasks;
using WebApp.Models;

namespace Microsoft.EntityFrameworkCore
{
    public interface ITenantAwareDbContext : IDisposable
    {
        DbSet<County> Counties { get; set; }
        DbContext DbContext { get; }
        DbSet<State> States { get; set; }
        Task<int> SaveChangesAsync();
    }
}