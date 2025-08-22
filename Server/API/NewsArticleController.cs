using AutoMapper;
using Entity.ModelResponse;
using Microsoft.AspNetCore.OData.Query;

namespace Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsArticleController : ControllerBase
    {
        private readonly INewsArticleRepository _repository;

        public NewsArticleController(INewsArticleRepository repository)
        {
                  _repository= repository;
        }

        [EnableQuery]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsArticleResponse>>> Get()
        {
               var data=await _repository.GetAllAsync();
               if(!data.Any())
               {
                   return NotFound();
               }
               return Ok(data);
        }
    }
}
