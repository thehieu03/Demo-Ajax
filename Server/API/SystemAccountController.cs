using Entity.ModelRequest;

namespace Server.API
{
    [Route("api/account")]
    [ApiController]
    public class SystemAccountController : ControllerBase
    {
        private readonly ISystemAccountRepository _systemAccountRepository;

        public SystemAccountController(ISystemAccountRepository systemAccountRepository)
        {
            _systemAccountRepository = systemAccountRepository;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SystemAccount>> Login([FromBody] LoginRequest login)
        {
            try
            {
                var result = await _systemAccountRepository.GetAccountByEmailOrPassword(login.Email, login.Password);
                return Ok(result);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}