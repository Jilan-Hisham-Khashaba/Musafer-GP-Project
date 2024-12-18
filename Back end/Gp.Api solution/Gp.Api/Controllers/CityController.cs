using AutoMapper;
using Gp.Api.Dtos;
using Gp.Api.Errors;
using GP.Core.Entities;
using GP.Core.Repositories;
using GP.Core.Specificatios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gp.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ApiBaseController
    {
        private readonly IGenericRepositroy<City> cityRepo;
        private readonly IMapper mapper;

        public CityController(IGenericRepositroy<City> CityRepo, IMapper mapper)
        {
            cityRepo = CityRepo;
            this.mapper = mapper;
        }
        [ProducesResponseType(typeof(CityToDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CityToDto>>> GetCity()
        {


            var spec = new CitySpecification();
           var cities = await cityRepo.GetAllWithSpecAsyn(spec);

            if (cities is null) return NotFound(new ApiResponse(404));
            var mappedCities = mapper.Map<IEnumerable<City>, IEnumerable<CityToDto>>(cities);
            return Ok(mappedCities);
        }
    }
}
