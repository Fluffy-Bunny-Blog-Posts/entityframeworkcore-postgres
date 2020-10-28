using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApp.Models;

namespace Microsoft.EntityFrameworkCore
{
    public class AppEntityCoreContext : DbContext, IAppEntityCoreContext
    {
        public AppEntityCoreContext(DbContextOptions<AppEntityCoreContext> options) : base(options) { }
        public DbSet<State> States { get; set; }
        public DbSet<County> Counties { get; set; }

        public DbContext DbContext => this;

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }
    }
}
