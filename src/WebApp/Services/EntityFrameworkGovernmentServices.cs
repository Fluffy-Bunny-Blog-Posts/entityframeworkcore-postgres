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
        
        public async Task UpsertStateAsync(State state)
        {
            var utcNow = DateTime.UtcNow;
            var query = from item in _context.States
                        where item.Name == state.Name
                        select item;
            var stateInDb = query.FirstOrDefault();
            if (stateInDb == null)
            {
                state.Id = GuidS;
                state.Created = utcNow;
                state.Updated = utcNow;
                _context.States.Add(state);
            }
            else
            {
                // now we update.
                stateInDb.Name = state.Name;
                stateInDb.Updated = utcNow;
                stateInDb.Abbreviation = state.Abbreviation;
            }
           

            // leaving counties alone for now.

            //    _context.States.Update(stateInDb);
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

        public async Task<IEnumerable<State>> GetStatesAsync(SortDirection sortOrder)
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

       

     
        public async Task UpsertCountyAsync(string stateId, County county)
        {
            var utcNow = DateTime.UtcNow;
            var query = from item in _context.States
                        where item.Id == stateId
                        select item;
            var stateInDb = query.FirstOrDefault();
            if (stateInDb == null)
            {
                throw new Exception($"stateId:{stateId} does not exist");
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

        public async Task<County> GetCountyByNameAsync(string stateId, string countyName)
        {
            var query = from item in _context.Counties
                        where item.StateFK == stateId && item.Name == countyName
                        select item;
            var countyInDb = query.FirstOrDefault();
            if (countyInDb == null)
            {
                throw new Exception($"stateId={stateId}, countyName={countyName} does not exist");
            }
            return countyInDb;
        }
        public async Task<List<County>> GetCountiesAsync(string stateId)
        {
            var query = from item in _context.Counties
                        where item.StateFK == stateId
                        select item;
            return query.ToList();
        }

        public async Task UpsertCityAsync(string stateId, string countyId, City city)
        {
            var utcNow = DateTime.UtcNow;
            var query = from item in _context.Counties
                        where item.StateFK == stateId && item.Id == countyId
                        select item;
            var countyInDb = query.FirstOrDefault();
            if (countyInDb == null)
            {
                throw new Exception($"stateId={stateId},countyId={countyId} does not exist");
            }

            var cities = new List<City>();
            if (countyInDb.Cities == null)
            {
                countyInDb.Cities = cities;
            };

            var query3 = from item in countyInDb.Cities
                         where item.Name == city.Name
                         select item;
            var cityInDb = query3.FirstOrDefault();
            if (cityInDb == null)
            {
                city.Id = GuidS;
                city.Created = utcNow;
                city.Updated = utcNow;
                city.StateFK = countyInDb.StateFK;
                countyInDb.Cities.Add(city);
            }
            else
            {
                cityInDb.Name = city.Name;
                cityInDb.Updated = utcNow;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<City>> GetCitiesAsync(string stateId, string countyId)
        {
            var query = from item in _context.States
                        where item.Id == stateId
                        select item;
            var stateInDb = query.FirstOrDefault();
            if (stateInDb == null)
            {
                throw new Exception($"stateId:{stateId} does not exist");
            }

            var query2 = from item in stateInDb.Counties
                         where item.Id == countyId
                         select item;
            var countyInDb = query2.FirstOrDefault();
            if (countyInDb == null)
            {
                throw new Exception($"stateId={stateId}, countyId={countyId} does not exist");
            }
            if (countyInDb.Cities == null)
            {
                return new List<City>();
            }
            var query3 = from item in countyInDb.Cities
                         select item;
            return query3.ToList();

        }

        public async Task<City> GetCityByIdAsync(string stateId, string countyId, string id)
        {
            var query = from item in _context.Cities
                        where item.StateFK == stateId && item.CountyFK == countyId && item.Id == id
                        select item;

            var cityInDb = query.FirstOrDefault();
            return cityInDb;
        }

    }

}
