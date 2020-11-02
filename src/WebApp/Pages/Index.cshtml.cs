using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApp.Services;

namespace WebApp.Pages
{
    public class IndexModel : PageModel
    {
        private string GuidS => Guid.NewGuid().ToString();
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAppEntityCoreContext _appEntityCoreContext;
        private readonly ITenantAwareDbContextAccessor _tenantAwareDbContextAccessor;
        private readonly IGovernmentServices _governmentServices;
        private readonly ILogger<IndexModel> _logger;
        
        public class TenantModel
        {
            [Required]
            [Display(Name = "Tenant")]
            public string TenantId { get; set; }
        }
        
        [BindProperty]
        public TenantModel InputTenant { get; set; }

        public IndexModel(
            IHttpContextAccessor httpContextAccessor,
            IAppEntityCoreContext appEntityCoreContext,
            ITenantAwareDbContextAccessor tenantAwareDbContextAccessor,
            IGovernmentServices governmentServices,
            ILogger<IndexModel> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _appEntityCoreContext = appEntityCoreContext;
            _tenantAwareDbContextAccessor = tenantAwareDbContextAccessor;
            _governmentServices = governmentServices;
            _logger = logger;
        }

        public async Task OnGetAsync()
        {

        }
        public async Task<IActionResult> OnPostEnsureAppContextAsync()
        {
            var utcNow = DateTime.UtcNow;


            var dbContext = _appEntityCoreContext.DbContext;
            var dbExists = await _appEntityCoreContext.DbContext.Database.CanConnectAsync();
            if (!dbExists)
            {
                // do stuff
                await dbContext.Database.MigrateAsync();
            }
            var state = new Models.State
            {
                Id = GuidS,
                Created = utcNow,
                Updated = utcNow,
                Name = "New California",
                Abbreviation = "NCA"
            };
            await _governmentServices.UpsertStateAsync(state);
            var ori = await _governmentServices.GetStateByAbbreviationAsync(state.Abbreviation);
            await _governmentServices.DeleteStateAsync(ori.Id);

            
            return RedirectToPage("./Index");
        }

            
        public async Task<IActionResult> OnPostSetTenantAsync()
        {
            var utcNow = DateTime.UtcNow;
            HttpContext.Session.SetString(".tenantId", InputTenant.TenantId);

            using (var tenantContext = _tenantAwareDbContextAccessor.GetTenantAwareDbContext(InputTenant.TenantId))
            {
                var dbContext = tenantContext.DbContext;
                var tenantDbExists = await dbContext.Database.CanConnectAsync();
                if (!tenantDbExists)
                {
                    // do stuff
                    await dbContext.Database.MigrateAsync();
                    tenantContext.States.Add(new Models.State
                    {
                        Id = GuidS,
                        Created = utcNow,
                        Updated = utcNow,
                        Name = "New California",
                        Abbreviation = "NCA"
                    });
                    await tenantContext.SaveChangesAsync();
                }
                var stat = from s in tenantContext.States
                           select s;
            }

            return RedirectToPage("./Index");

        }
    }
}
