using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebApp.Models;

namespace Microsoft.EntityFrameworkCore
{
    public interface IAppEntityCoreContext
    {
        DbSet<County> Counties { get; set; }
        DbSet<State> States { get; set; }
        DbContext DbContext { get; }
        Task<int> SaveChangesAsync();
    }
}