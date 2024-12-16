using GP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Core.Repositories
{
    public interface ICountryRepository : IGenericRepositroy<Country>
    {
        Task<Country> GetCountryByNameAsync(string countryName);
        // يمكنك إضافة المزيد من الوظائف حسب احتياجاتك للدولة
    }
}
