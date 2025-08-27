namespace Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public CategoryController(ICategoryRepository categoryRepository,IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper; 
        }
        [HttpGet]
        [Produces("application/json")]
        [EnableQuery]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CategoryResponse>>> Get()
        {
            var data = await _categoryRepository.GetAllAsync();
            if (!data.Any())
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpGet("{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CategoryResponse>> Get(short id)
        {
            var data = await _categoryRepository.GetById(id);
            var categoroyResponse=_mapper.Map<CategoryResponse>(data);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(categoroyResponse);
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryResponse>> Post([FromBody] CategoryAdd category)
        {
            var newCategory = _mapper.Map<Category>(category);
            await _categoryRepository.Create(newCategory);
            var data = _mapper.Map<CategoryResponse>(newCategory);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryResponse>> Put(short id, [FromBody] CategoryAdd category)
        {
            var existing = await _categoryRepository.GetById(id);
            if (existing == null)
            {
                return NotFound();
            }
            var updated = _mapper.Map<Category>(category);
            updated.CategoryId = id;
            await _categoryRepository.Update(updated);
            var data = _mapper.Map<CategoryResponse>(updated);
            return Ok(data);
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(short id)
        {
            var existingCategory = await _categoryRepository.GetById(id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            await _categoryRepository.Delete(id);
            return Ok();
        }
    }
}
