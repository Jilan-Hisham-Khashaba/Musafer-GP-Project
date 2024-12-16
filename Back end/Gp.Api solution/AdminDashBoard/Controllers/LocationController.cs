using AdminDashBoard.Models;
using AutoMapper;
using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Core.Specificatios;
using GP.Repository;
using Microsoft.AspNetCore.Mvc;

namespace AdminDashBoard.Controllers
{
    public class LocationController : Controller
    {
        private readonly IGenericRepositroy<City> citiesRepo;
        private readonly IGenericRepositroy<Country> countryRepo;
        private readonly IMapper mapper;

        public LocationController(IGenericRepositroy<City> citiesRepo,IGenericRepositroy<Country> countryRepo,IMapper mapper)
        {
            this.citiesRepo = citiesRepo;
            this.countryRepo = countryRepo;
            this.mapper = mapper;
        }
        public async Task<IActionResult> Index(string searchValue)
        {
            var spec = new CitySpecification();
            var cities = await citiesRepo.GetAllWithSpecAsyn(spec);
            var CityList = new List<CitiesModel>();

            foreach (var city in cities)
            {
                var citiesData = new CitiesModel
                {
                    Id = city.Id,
                    cityName = city.NameOfCity,
                    countryId = city.CountryId,
                    countryName = city.Country.NameCountry,
                    contient = city.Country.Contient,
                };

                CityList.Add(citiesData);
            }

            // إذا كان searchValue غير فارغ، قم بتصفية القائمة بناءً على searchValue
            if (!string.IsNullOrEmpty(searchValue))
            {
                CityList = CityList.Where(c => c.cityName.Contains(searchValue, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(CityList);
        }

        public IActionResult AddCity()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddCity(CitiesModel city)
        {
            if (!string.IsNullOrEmpty(city.cityName) && city.countryName != null)
            {
                // يُفضل التحقق من عودة الدالة AddAsync لتأكيد إضافة الفئة بنجاح
                // إضافة الدولة إذا كانت غير موجودة
                var country = await countryRepo.GetByNameAsync(city.countryName);
                if (country == null)
                {
                    // إضافة الدولة إذا كانت غير موجودة
                    await countryRepo.AddAsync(new Country { NameCountry = city.countryName });
                }

                // إضافة المدينة مع معرف الدولة
                var citiesEntity = mapper.Map<City>(city);
                await citiesRepo.AddAsync(citiesEntity);
                await citiesRepo.SaveChangesAsync();

                TempData["SuccessMessage"] = "Location added successfully!";
                return RedirectToAction("Index"); // أعد التوجيه إلى الصفحة المناسبة بعد الإضافة بنجاح
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to add Location!";
            }

            // إذا كان هناك أي أخطاء في النموذج، أو إذا فشلت عملية الإضافة
            // سنعود مباشرة إلى نموذج الإضافة لعرض الأخطاء للمستخدم
            return View(city);
        }
        [HttpGet]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // احصل على الفئة المراد تعديلها من الريبوزيتوري
            var city = await citiesRepo.GetByIdAsync(id.Value);
            if (city == null)
            {
                return NotFound();
            }

            // احصل على بيانات الدولة المرتبطة مع المدينة
            var country = await countryRepo.GetByIdAsync(city.CountryId);
            if (country == null)
            {
                return NotFound();
            }

            // قم بتعديل الفئة CityModel لتضمين بيانات الدولة
            var cityModel = new CitiesModel
            {
                Id = city.Id,
                cityName = city.NameOfCity,
                countryId = city.CountryId,
                countryName = country.NameCountry, // او حسب الخاصية المناسبة في كلاس CountriesModel
                contient = country.Contient // او حسب الخاصية المناسبة في كلاس CountriesModel
            };

            return View(cityModel);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, CitiesModel cities)
        {
            if (id != cities.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the city entity
                    var cityEntity = await citiesRepo.GetByIdAsync(id);
                    if (cityEntity == null)
                    {
                        return NotFound();
                    }

                    // Fetch the related country entity using the city's country ID
                    var countryEntity = await countryRepo.GetByIdAsync(cityEntity.CountryId);
                    if (countryEntity == null)
                    {
                        return NotFound();
                    }

                    // Update city entity's name
                    cityEntity.NameOfCity = cities.cityName;

                    // Update country entity's details
                    countryEntity.NameCountry = cities.countryName;
                    countryEntity.Contient = cities.contient;

                    // Attach and mark entities as modified
                    citiesRepo.Update(cityEntity);
                    countryRepo.Update(countryEntity);

                    // Save changes
                    await citiesRepo.SaveChangesAsync();
                    await countryRepo.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Location updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Log the error and display an error message
                    ModelState.AddModelError(string.Empty, $"An error occurred while updating the location. Please try again. Error: {ex.Message}");
                    return View(cities);
                }
            }

            // If the model state is invalid, redisplay the form with errors
            return View(cities);
        }


        public ActionResult Delete(int id)
        {
            var category = citiesRepo.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);

        }
        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            
            var city = await citiesRepo.GetByIdAsync(id);
            if (city == null)
            {
                return NotFound();
            }
            var country = await countryRepo.GetByIdAsync(city.CountryId);
            if (country == null)
            {
                return NotFound();
            }

            try
            {
               
                await citiesRepo.DeleteAsync(city.Id);

                await countryRepo.DeleteAsync(country.Id);

                TempData["SuccessMessage"] = "Location deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction(nameof(Index));
            }
        }




    }
}
