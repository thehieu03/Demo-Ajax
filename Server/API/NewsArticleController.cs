using Microsoft.AspNetCore.Authorization;
using Entity.ModelRequest;

namespace Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsArticleController : ControllerBase
    {
        private readonly INewsArticleRepository _repository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public NewsArticleController(INewsArticleRepository repository, ICategoryRepository categoryRepository, ITagRepository tagRepository, IMapper mapper)
        {
            _repository = repository;
            _categoryRepository = categoryRepository;
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [EnableQuery]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsArticleResponse>>> Get()
        {
            var data = await _repository.GetAllAsync();
            if (!data.Any())
            {
                return NotFound();
            }
            var orderedData = data.OrderByDescending(x => x.NewsArticleId);
            return Ok(orderedData);
        }

        [HttpPost("create")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NewsArticleResponse>> Create([FromBody] CreateNewsArticleRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.CategoryId.HasValue)
            {
                var category = await _categoryRepository.GetById(request.CategoryId.Value);
                if (category == null)
                {
                    return BadRequest($"Category with ID {request.CategoryId} does not exist");
                }
            }

            if (request.TagIds != null && request.TagIds.Any())
            {
                foreach (var tagId in request.TagIds)
                {
                    var tag = await _tagRepository.GetById(tagId);
                    if (tag == null)
                    {
                        return BadRequest($"Tag with ID {tagId} does not exist");
                    }
                }
            }

            var userId = User.GetAccountId();
            if (userId < 0)
            {
                return BadRequest("Invalid user ID in token");
            }

            var newsArticle = new NewsArticle
            {
                NewsTitle = request.NewsTitle,
                Headline = request.Headline,
                NewsContent = request.NewsContent,
                NewsSource = request.NewsSource,
                CategoryId = request.CategoryId,
                NewsStatus = request.NewsStatus,
                CreatedById = userId,
                CreatedDate = DateTime.Now,
                ModifiedDate = DateTime.Now
            };

            if (request.TagIds != null && request.TagIds.Any())
            {
                var tags = new List<Tag>();
                foreach (var tagId in request.TagIds)
                {
                    var tag = await _tagRepository.GetById(tagId);
                    if (tag != null)
                    {
                        tags.Add(tag);
                    }
                }
                newsArticle.Tags = tags;
            }

            var createdArticle = await _repository.AddAsync(newsArticle);
            var response = _mapper.Map<NewsArticleResponse>(createdArticle);
            
            return Ok(response);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(string id)
        {
            var existing = await _repository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }
            await _repository.Delete(id);
            return Ok();
        }
    }
}
