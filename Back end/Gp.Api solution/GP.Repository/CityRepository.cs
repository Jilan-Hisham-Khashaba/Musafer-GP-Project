using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Repository.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Repository
{
    public class CityRepository : GenericRepositorty<City>, ICityRepository
    {
        private readonly StoreContext dbContext;
        public CityRepository(StoreContext dbContext) :base(dbContext)
        {
            this.dbContext = dbContext;
        }
        

        public async Task<City> GetCityByNameAsync(string cityName)
        {
            return await dbContext.City.FirstOrDefaultAsync(c => c.NameOfCity == cityName);
        }

        
    }
}
