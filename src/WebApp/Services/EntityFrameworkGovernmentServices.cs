using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using WebApp.Models;

namespace WebApp.Services
{
    public class EntityFrameworkGovernmentServices : IGovernmentServices
    {
        private string GuidS => Guid.NewGuid().ToString();
        private string GuidN => Guid.NewGuid().ToString("N");

        private IAppEntityCoreContext _context;
        private ILogger<EntityFrameworkGovernmentServices> _logger;

        public EntityFrameworkGovernmentServices(
            IAppEntityCoreContext context,
            ILogger<EntityFrameworkGovernmentServices> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task AddStateAsync(State state)
        {
            var ori = await GetStateByNameAsync(state.Name);
            if (ori != null)
                return;
            state.Id = GuidS;
            _context.States.Add(state);
            await _context.SaveChangesAsync();
        }

        

        public async Task DeleteStateAsync(string id)
        {
            // this gets the real entity from the States set
            var entity = await _context.States.FindAsync(id);

            /*
             * https://github.com/dotnet/efcore/issues/12459
             
            An unhandled exception has occurred while executing the request.
            System.InvalidOperationException: The instance of entity type 'State' cannot be tracked because another instance with the same key value for {'Id'} is already being tracked. When attaching existing entities, ensure that only one entity instance with a given key value is attached.

            // this is not the same entity, and just a hint.
            entity = new State
                        {
                            Id = id
                        };

            */

            if (entity == null)
            {
                return;
            }
            _context.States.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<State>> GetAllStatesAsync(SortDirection sortOrder)
        {
            var states = from s in _context.States
                         select s;
            switch (sortOrder)
            {
                case SortDirection.Descending:
                    states = states.OrderByDescending(s => s.Name);
                    break;

                case SortDirection.Ascending:
                    states = states.OrderBy(s => s.Name);
                    break;
                case SortDirection.None:
                default:
                    break;

            }
            return states.AsNoTracking();
        }

        public async Task<PaginatedList<State>> GetPageStatesAsync(
            int? pageNumber, int pageSize, string sortField, SortDirection sortOrder)
        {
            var stateList = _context.States.OrderByDynamic(sortField, sortOrder);
            return await PaginatedList<State>.CreateAsync(stateList.AsNoTracking(), pageNumber ?? 1, pageSize);

        }

        public async Task<State> GetStateByAbbreviationAsync(string abbreviation)
        {
            var query = from item in _context.States
                        where item.Abbreviation == abbreviation
                        select item;
            return query.FirstOrDefault();
        }

        public async Task<State> GetStateByIdAsync(string id)
        {
            var query = from item in _context.States
                        where item.Id == id
                        select item;
            return query.FirstOrDefault();
        }

        public async Task<State> GetStateByNameAsync(string name)
        {
            var query = from item in _context.States
                        where item.Name == name
                        select item;
            return query.FirstOrDefault();
        }

        public async Task UpdateStateAsync(State state)
        {
            var query = from item in _context.States
                        where item.Id == state.Id
                        select item;
            var stateInDb = query.FirstOrDefault();
            if(stateInDb == null)
            {
                await AddStateAsync(state);
                return;
            }
            // now we update.
            var utcNow = DateTime.UtcNow;
            stateInDb.Name = state.Name;
            stateInDb.Updated = utcNow;
            stateInDb.Abbreviation = state.Abbreviation;

            // leaving counties alone for now.

        //    _context.States.Update(stateInDb);
            await _context.SaveChangesAsync();
        }

     
        public async Task UpsertCountyAsync(string stateAbbreviation, County county)
        {
            var utcNow = DateTime.UtcNow;
            var query = from item in _context.States
                        where item.Abbreviation == stateAbbreviation
                        select item;
            var stateInDb = query.FirstOrDefault();
            if (stateInDb == null)
            {
                throw new Exception($"State:{stateAbbreviation} does not exist");
            }
            var counties = new List<County>();
            if (stateInDb.Counties == null)
            {
                stateInDb.Counties = counties;
            };

            var query2 = from item in stateInDb.Counties
                        where item.Name == county.Name
                    select item;
            var countyInDb = query2.FirstOrDefault();
            if(countyInDb == null)
            {
                county.Id = GuidS;
                county.Created = utcNow;
                county.Updated = utcNow;
                stateInDb.Counties.Add(county);
            }
            else
            {
                countyInDb.Name = county.Name;
                countyInDb.Updated = utcNow;
            }
            await _context.SaveChangesAsync();

        }

        public async Task<List<County>> GetCounties(string stateAbbreviation)
        {
            var query = from item in _context.States
                        where item.Abbreviation == stateAbbreviation
                        select item;
            var stateInDb = query.FirstOrDefault();
            if (stateInDb == null)
            {
                throw new Exception($"State:{stateAbbreviation} does not exist");
            }
            var query2 = from item in stateInDb.Counties
                         select item;
            return query2.ToList();
        }
    }

}
