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
    public class CountryRepository : GenericRepositorty<Country>, ICountryRepository
    {
        private readonly StoreContext dbContext;
        public CountryRepository(StoreContext dbContext) :base(dbContext) 
        {
            this.dbContext = dbContext;
        }

        
        public async Task<Country> GetCountryByNameAsync(string countryName)
        {
            return await dbContext.Country.FirstOrDefaultAsync(c => c.NameCountry == countryName);
         
        }
    }
}
