using GP.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GP.Repository
{
    public class NameToIdResolver : INameToIdResolver
    {
        private readonly ICityRepository cityRepository; // قم بتعديل هذا وفقًا لتكوين تطبيقك
        private readonly ICountryRepository countryRepository; // قم بتعديل هذا وفقًا لتكوين تطبيقك

        public NameToIdResolver(ICityRepository cityRepository, ICountryRepository countryRepository)
        {
            this.cityRepository = cityRepository;
            this.countryRepository = countryRepository;
        }
        public async Task<int> ResolveCityIdByNameAsync(string cityName)
        {
            var city = await cityRepository.GetCityByNameAsync(cityName);
            return city?.Id ?? 0; // قد تعيد 0 إذا لم يتم العثور على المدينة
        }

        public async Task<int> ResolveCountryIdByNameAsync(string countryName)
        {
            var country = await countryRepository.GetCountryByNameAsync(countryName);
            return country?.Id ?? 0;
        }
    }
}
