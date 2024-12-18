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
    public class CategoriesController : ApiBaseController
    {
        private readonly IGenericRepositroy<Category> categoryRepo;
        private readonly IMapper mapper;

        public CategoriesController(IGenericRepositroy<Category> CategoryRepo, IMapper mapper)
        {
            categoryRepo = CategoryRepo;
            this.mapper = mapper;
        }
        [ProducesResponseType(typeof(CategoriesToDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriesToDto>>> GetCategory()
        {
            var category= await categoryRepo.GetAllAsync();
            if (category is null) return NotFound(new ApiResponse(404));
            var mappedCategory = mapper.Map<IEnumerable<Category>, IEnumerable<CategoriesToDto>>(category);

            return Ok(mappedCategory);
        }
    }
}
