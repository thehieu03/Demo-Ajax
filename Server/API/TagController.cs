using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Entity.ModelRequest;
using Entity.Models;

namespace Server.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;

        public TagController(ITagRepository tagRepository, IMapper mapper)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<TagResponse>>> Get()
        {
            var data = await _tagRepository.GetAllAsync();
            if (!data.Any())
            {
                return NotFound();
            }
            var response = _mapper.Map<IEnumerable<TagResponse>>(data);
            return Ok(response);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TagResponse>> Create([FromBody] TagAdd tagAdd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tag = _mapper.Map<Tag>(tagAdd);
                await _tagRepository.Create(tag);
                
                var response = _mapper.Map<TagResponse>(tag);
                return CreatedAtAction(nameof(Get), new { id = response.TagId }, response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi tạo tag: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TagResponse>> Update(int id, [FromBody] TagAdd tagAdd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existingTag = await _tagRepository.GetById(id);
                if (existingTag == null)
                {
                    return NotFound("Tag không tồn tại");
                }

                existingTag.TagName = tagAdd.TagName;
                existingTag.Note = tagAdd.Note;

                await _tagRepository.Update(existingTag);
                
                var response = _mapper.Map<TagResponse>(existingTag);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest($"Lỗi khi cập nhật tag: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                Console.WriteLine($"[DELETE_TAG] Bắt đầu xóa tag với ID: {id}");
                
                var existingTag = await _tagRepository.GetById(id);
                if (existingTag == null)
                {
                    Console.WriteLine($"[DELETE_TAG] Tag với ID {id} không tồn tại");
                    return NotFound("Tag không tồn tại");
                }

                Console.WriteLine($"[DELETE_TAG] Tìm thấy tag: {existingTag.TagName}");
                Console.WriteLine($"[DELETE_TAG] Số bài viết liên quan: {existingTag.NewsArticles?.Count ?? 0}");
                
                // Thông báo cho user biết sẽ xóa cả liên kết
                if (existingTag.NewsArticles != null && existingTag.NewsArticles.Any())
                {
                    Console.WriteLine($"[DELETE_TAG] Tag đang được sử dụng trong {existingTag.NewsArticles.Count} bài viết, sẽ xóa cả liên kết");
                }
                
                Console.WriteLine($"[DELETE_TAG] Bắt đầu gọi repository.Delete({id})");
                
                await _tagRepository.Delete(id);
                
                Console.WriteLine($"[DELETE_TAG] Xóa tag thành công");
                return Ok("Tag đã được xóa thành công cùng với tất cả liên kết bài viết");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DELETE_TAG] Lỗi khi xóa tag: {ex.Message}");
                Console.WriteLine($"[DELETE_TAG] Stack trace: {ex.StackTrace}");
                
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[DELETE_TAG] Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"[DELETE_TAG] Inner stack trace: {ex.InnerException.StackTrace}");
                }
                
                return BadRequest($"Lỗi khi xóa tag: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TagResponse>> GetById(int id)
        {
            var tag = await _tagRepository.GetById(id);
            if (tag == null)
            {
                return NotFound("Tag không tồn tại");
            }

            var response = _mapper.Map<TagResponse>(tag);
            return Ok(response);
        }
    }
}
