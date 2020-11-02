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
using WebApp.Models;
using System.Collections.Specialized;
using System.Linq;

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
                    Created = utcNow,
                    Updated = utcNow,
                    Name = "New California",
                    Abbreviation = "NCA"
                };

                Func<Task> func = async () => { await governmentServices.UpsertStateAsync(state); };
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
        public async Task CRUD_IGovernmentServices_Update_State_Success_Async()
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
                    Created = utcNow,
                    Updated = utcNow,
                    Name = "Old California",
                    Abbreviation = "OCA"
                };

                Func<Task> func = async () => 
                { 
                    await governmentServices.UpsertStateAsync(state); 
                };
                func.Should().NotThrow();


                var ori = await governmentServices.GetStateByAbbreviationAsync(state.Abbreviation);
                ori.Should().NotBeNull();
                ori.Abbreviation.Should().Be(state.Abbreviation);

                ori.Name = "New Oregon";
                ori.Abbreviation = "NOR";

                func = async () =>
                {
                    await governmentServices.UpsertStateAsync(ori);
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
        [Fact]
        public async Task CRUD_IGovernmentServices_Update_County_Success_Async()
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
                    Created = utcNow,
                    Updated = utcNow,
                    Name = "Old California",
                    Abbreviation = "OCA"
                };

                Func<Task> func = async () =>
                {
                    await governmentServices.UpsertStateAsync(state);
                };
                func.Should().NotThrow();


                var stateInDb = await governmentServices.GetStateByAbbreviationAsync(state.Abbreviation);
                stateInDb.Should().NotBeNull();
                stateInDb.Abbreviation.Should().Be(state.Abbreviation);

                var county = new County
                {
                    Name = "Toole"
                };
                func = async () =>
                {
                    await governmentServices.UpsertCountyAsync(stateInDb.Id,county);
                };
                func.Should().NotThrow();
                var counties = await governmentServices.GetCountiesAsync(stateInDb.Id);
                counties.Should().NotBeNull();
                counties.Count.Should().Be(1);
                var countyInDb = counties.FirstOrDefault();
                countyInDb.Name.Should().Be(county.Name);
                func = async () =>
                {
                    countyInDb.Name = "Silver Bow";
                    await governmentServices.UpsertCountyAsync(stateInDb.Id, countyInDb);
                };
                func.Should().NotThrow();
                counties = await governmentServices.GetCountiesAsync(stateInDb.Id);
                counties.Should().NotBeNull();
                counties.Count.Should().Be(1);
                var countyInDb2 = counties.FirstOrDefault();
                countyInDb2.Name.Should().Be(countyInDb.Name);


                func = async () => { await governmentServices.DeleteStateAsync(stateInDb.Id); };
                func.Should().NotThrow();

                stateInDb = await governmentServices.GetStateByAbbreviationAsync(stateInDb.Abbreviation);
                stateInDb.Should().BeNull();

            }
        }
        [Fact]
        public async Task CRUD_IGovernmentServices_Update_City_Success_Async()
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
                    Created = utcNow,
                    Updated = utcNow,
                    Name = "Old California",
                    Abbreviation = "OCA"
                };
                var county = new County
                {
                    Name = "Toole"
                };
                var city = new City
                {
                    Name = "Sunburst"
                };

                Func<Task> func = async () =>
                {
                    await governmentServices.UpsertStateAsync(state);
                };
                func.Should().NotThrow();
                var stateInDb = await governmentServices.GetStateByAbbreviationAsync(state.Abbreviation);

                func = async () =>
                {
                    await governmentServices.UpsertCountyAsync(stateInDb.Id,county);
                };
                func.Should().NotThrow();
                var countyInDb = await governmentServices.GetCountyByNameAsync(stateInDb.Id, county.Name);
                countyInDb.Should().NotBeNull();

                func = async () =>
                {
                    await governmentServices.UpsertCityAsync(
                        stateInDb.Id,
                        countyInDb.Id, 
                        city);
                };
                func.Should().NotThrow();

                var cities = await governmentServices
                    .GetCitiesAsync(stateInDb.Id, countyInDb.Id);
                cities.Should().NotBeNull();
                cities.Count().Should().Be(1);

                var cityInDb = await governmentServices.GetCityByIdAsync(stateInDb.Id,
                    countyInDb.Id, cities[0].Id);
                cityInDb.Should().NotBeNull();
                cityInDb.Name.Should().Be(city.Name);

                cityInDb.Name = "Sweetgrass";
                func = async () =>
                {
                    await governmentServices.UpsertCityAsync(
                        stateInDb.Id,
                        countyInDb.Id,
                        cityInDb);
                };
                func.Should().NotThrow();

                var cityInDb2 = await governmentServices.GetCityByIdAsync(
                    stateInDb.Id,
                    countyInDb.Id,
                    cityInDb.Id);
                cityInDb2.Should().NotBeNull();
                cityInDb2.Name.Should().Be(cityInDb.Name);


 
                func = async () => { await governmentServices.DeleteStateAsync(stateInDb.Id); };
                func.Should().NotThrow();

                stateInDb = await governmentServices.GetStateByAbbreviationAsync(stateInDb.Abbreviation);
                stateInDb.Should().BeNull();

            }
        }
    }
}
