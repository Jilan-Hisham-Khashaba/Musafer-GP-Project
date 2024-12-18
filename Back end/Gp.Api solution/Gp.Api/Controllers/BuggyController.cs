using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Gp.Api.Errors;
using GP.Repository.Data;

namespace Gp.Api.Controllers
{

    public class BuggyController : ApiBaseController
    {
        private readonly StoreContext _dbContext;

        public BuggyController(StoreContext dbContext) 
        {
            _dbContext = dbContext;
        }

        [HttpGet("notfound")]  // GET : api/buggy/notfound
        public ActionResult GetNotFoundRequest()
        {
            var product = _dbContext.Products.Find(200); 
            
            if(product == null) 
                return NotFound(new ApiResponse(404));

            return Ok(product);
        }

        [HttpGet("servererror")]  // GET : api/buggy/servererror
        public ActionResult GetServerError()
        {
            var product = _dbContext.Products.Find(100);
            var productToReturn = product.ToString();  // Will Throw Exception [NullReferencrExcetion]   

            return Ok(productToReturn);
        }

        [HttpGet("badrequest")]  // GET : api/buggy/badrequest
        public ActionResult GetBadRequest()
        {
            return BadRequest(new ApiResponse(400));
        }

        [HttpGet("badrequest/{id}")]  // GET : api/buggy/badrequest/five
        public ActionResult GetBadRequest(int id)  // Validation Error
        {
            return Ok();
        }

        // T2reban nasr mas7a fi el video
        //[HttpGet("unauthorized")]  // GET : api/buggy/unauthorized
        //public ActionResult GetunauthorizedError(int id)
        //{
        //    return Unauthorized(new ApiResponse(401));
        //}
    }
}
