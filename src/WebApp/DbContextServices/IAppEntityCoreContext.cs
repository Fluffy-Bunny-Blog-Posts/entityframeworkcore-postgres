using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApp.Models;

namespace Microsoft.EntityFrameworkCore
{
    public interface IAppEntityCoreContext
    {
        DbSet<State> States { get; set; }
        DbSet<County> Counties { get; set; }
        DbSet<City> Cities { get; set; }
        DbContext DbContext { get; }
        Task<int> SaveChangesAsync();
    }
}