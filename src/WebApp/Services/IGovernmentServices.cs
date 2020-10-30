using System.Collections.Generic;
using System.Threading.Tasks;
using WebApp.Models;

namespace WebApp.Services
{
    public interface IGovernmentServices
    {
        Task AddStateAsync(State state);
        Task UpdateStateAsync(State state);

        Task DeleteStateAsync(string id);
        Task<State> GetStateByIdAsync(string id);
        Task<State> GetStateByAbbreviationAsync(string abbreviation);
        Task<State> GetStateByNameAsync(string name);
        Task<IEnumerable<State>> GetAllStatesAsync(SortDirection sortOrder);
        Task<PaginatedList<State>> GetPageStatesAsync(int? pageNumber,
                                                      int pageSize,
                                                      string sortField,
                                                      SortDirection sortOrder);
    }
}
