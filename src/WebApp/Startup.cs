using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using WebApp.Models;
using WebApp.Services;

namespace WebApp
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostEnvironment HostEnvironment { get; }
        public Startup(
            IConfiguration configuration, 
            IHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;
            HostEnvironment = hostingEnvironment;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // Adds a default in-memory implementation of IDistributedCache.
            services.AddDistributedMemoryCache();
            services.AddSession();

            // the raw states data, we are going to transfer this into our entity database.
            services.Configure<List<StateModel>>(Configuration.GetSection("states"));

            services.Configure<AppOptions>(Configuration.GetSection("AppOptions"));

         
            
            services.AddDbContextTenantServices();

            services.AddEntityFrameworkNpgsql();

            services.Configure<EntityFrameworkConnectionOptions>(Configuration.GetSection("EntityFramworkConnectionOptions"));

            var useInMemoryEntityFramework = Configuration["AppOptions:UseInMemoryEntityFramework"];
            if (useInMemoryEntityFramework != "True")
            {
                // use postgress
                // this is only here so that migration models can be created.
                // we then use it as a template to not only create the new database for the tenant, but
                // downstream using it as a normal connection.
                services.AddDbContext<TenantAwareDbContext>((serviceProvider, optionsBuilder) => PostgresDelegates.DbContextConfigurationWithServiceProvider(serviceProvider, optionsBuilder));
                services.AddDbContext<AppEntityCoreContext>((serviceProvider, optionsBuilder) => PostgresDelegates.DbContextConfigurationWithServiceProvider(serviceProvider, optionsBuilder));
                services.AddPostgresDbContextOverrides();

            }
            else
            {
                services.AddDbContext<TenantAwareDbContext>((serviceProvider, optionsBuilder) => InMemoryDelegates.DbContextConfigurationWithServiceProvider(serviceProvider, optionsBuilder));
                services.AddDbContext<AppEntityCoreContext>((serviceProvider, optionsBuilder) => InMemoryDelegates.DbContextConfigurationWithServiceProvider(serviceProvider, optionsBuilder));
                services.AddInMemoryDbContextOverrides();
            }

            services.AddScoped<IAppEntityCoreContext, AppEntityCoreContext>();

            services.AddScoped<IGovernmentServices, EntityFrameworkGovernmentServices>();
            services.AddControllers();
            IMvcBuilder builder = services.AddRazorPages();
            if (HostEnvironment.IsDevelopment())
            {
                builder.AddRazorRuntimeCompilation();
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            
            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapRazorPages();
            });
        }
    }
}
