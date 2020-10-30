using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Configuration;
using System.Threading.Tasks;
using WebApp.Services;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace XUnitTest_InMemoryDbContext
{

    // https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1
    // https://wellsb.com/csharp/aspnet/xunit-unit-test-razor-pages/

    public class UnitTest_GovernmentServices :
        IClassFixture<CustomWebApplicationFactory<WebApp.Startup>>
    {
        private string GuidS => Guid.NewGuid().ToString();
        private readonly CustomWebApplicationFactory<WebApp.Startup> _factory;
        private readonly System.Net.Http.HttpClient _client;

        public UnitTest_GovernmentServices(CustomWebApplicationFactory<WebApp.Startup> factory)
        {
            _factory = factory;
            _client = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                });
            }).CreateClient();
        }
        [Fact]
        public async Task Ensure_TestEnvironment_Async()
        {
            var serviceProvider = _factory.Server.Services;
            using (var scope = serviceProvider.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var configuration = sp.GetRequiredService<IConfiguration>();
                configuration.Should().NotBeNull();
                /*
                  "AppOptions": {
                    "UseInMemoryEntityFramework": true
                  },
                */
 
                var useInMemoryEntityFramework = configuration["AppOptions:UseInMemoryEntityFramework"];
                string.IsNullOrWhiteSpace(useInMemoryEntityFramework).Should().BeFalse();
                useInMemoryEntityFramework.Should().Be("True");

            }
        }
        [Fact]
        public async Task Ensure_IGovernmentServices_Async()
        {
            var serviceProvider = _factory.Server.Services;
            using (var scope = serviceProvider.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var scopedService = sp.GetRequiredService<IGovernmentServices>();
                scopedService.Should().NotBeNull();

                var appEntityCoreContext = sp.GetRequiredService<IAppEntityCoreContext>();
                appEntityCoreContext.Should().NotBeNull();
            }
        }
        [Fact]
        public async Task CRUD_IGovernmentServices_Add_State_Delete_State_Success_Async()
        {
            var serviceProvider = _factory.Server.Services;
            using (var scope = serviceProvider.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var governmentServices = sp.GetRequiredService<IGovernmentServices>();
                governmentServices.Should().NotBeNull();

                var appEntityCoreContext = sp.GetRequiredService<IAppEntityCoreContext>();
                appEntityCoreContext.Should().NotBeNull();

                var dbContext = appEntityCoreContext.DbContext;
                var dbExists = await appEntityCoreContext.DbContext.Database.CanConnectAsync();
                dbExists.Should().BeTrue();
                
                var utcNow = DateTime.UtcNow;

                var state = new WebApp.Models.State
                {
                    Id = GuidS,
                    Created = utcNow,
                    Updated = utcNow,
                    Name = "New California",
                    Abbreviation = "NCA"
                };

                Func<Task> func = async () => { await governmentServices.AddStateAsync(state); };
                func.Should().NotThrow();


                var ori = await governmentServices.GetStateByAbbreviationAsync(state.Abbreviation);
                ori.Should().NotBeNull();
                ori.Abbreviation.Should().Be(state.Abbreviation);

                func = async () => { await governmentServices.DeleteStateAsync(ori.Id); };
                func.Should().NotThrow();

                ori = await governmentServices.GetStateByAbbreviationAsync(state.Abbreviation);
                ori.Should().BeNull();

            }
        }


        [Fact]
        public async Task CRUD_IGovernmentServices_Update_Success_Async()
        {
            var serviceProvider = _factory.Server.Services;
            using (var scope = serviceProvider.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var governmentServices = sp.GetRequiredService<IGovernmentServices>();
                governmentServices.Should().NotBeNull();

                var appEntityCoreContext = sp.GetRequiredService<IAppEntityCoreContext>();
                appEntityCoreContext.Should().NotBeNull();

                var dbContext = appEntityCoreContext.DbContext;
                var dbExists = await appEntityCoreContext.DbContext.Database.CanConnectAsync();
                dbExists.Should().BeTrue();

                var utcNow = DateTime.UtcNow;

                var state = new WebApp.Models.State
                {
                    Id = GuidS,
                    Created = utcNow,
                    Updated = utcNow,
                    Name = "Old California",
                    Abbreviation = "OCA"
                };

                Func<Task> func = async () => 
                { 
                    await governmentServices.AddStateAsync(state); 
                };
                func.Should().NotThrow();


                var ori = await governmentServices.GetStateByAbbreviationAsync(state.Abbreviation);
                ori.Should().NotBeNull();
                ori.Abbreviation.Should().Be(state.Abbreviation);

                ori.Name = "New Oregon";
                ori.Abbreviation = "NOR";

                func = async () =>
                {
                    await governmentServices.UpdateStateAsync(ori);
                };
                func.Should().NotThrow();
                var ori2 = await governmentServices
                                .GetStateByAbbreviationAsync(ori.Abbreviation);
                ori2.Should().NotBeNull();
                ori2.Abbreviation.Should().Be(ori.Abbreviation);


                func = async () => { await governmentServices.DeleteStateAsync(ori2.Id); };
                func.Should().NotThrow();

                ori = await governmentServices.GetStateByAbbreviationAsync(ori2.Abbreviation);
                ori.Should().BeNull();

            }
        }
    }
}
