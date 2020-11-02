using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services
{
    public interface IGovernmentServices
    {
         
        Task UpsertStateAsync(State state);
        Task<State> GetStateByIdAsync(string id);
        Task<State> GetStateByAbbreviationAsync(string abbreviation);
        Task<State> GetStateByNameAsync(string name);

        Task UpsertCityAsync(string stateId, string countyId, City city);
        Task<City> GetCityByIdAsync(string stateId, string countyId, string id);

        Task UpsertCountyAsync(string stateAbbreviation, County county);
        Task<County> GetCountyByNameAsync(string stateId, string countyName);
        Task<List<County>> GetCountiesAsync(string stateId);
        Task<List<City>> GetCitiesAsync(string stateId, string countyId);
        Task DeleteStateAsync(string id);

        Task<IEnumerable<State>> GetStatesAsync(SortDirection sortOrder);
        Task<PaginatedList<State>> GetPageStatesAsync(int? pageNumber,
                                                      int pageSize,
                                                      string sortField,
                                                      SortDirection sortOrder);
    }
}
