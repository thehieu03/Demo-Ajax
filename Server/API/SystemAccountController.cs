using System.IdentityModel.Tokens.Jwt;
using Entity.ModelResponse;

namespace Server.API
{
    [Route("api/account")]
    [ApiController]
    public class SystemAccountController : ControllerBase
    {
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly IConfiguration _configuration;

        public SystemAccountController(ISystemAccountRepository systemAccountRepository, IConfiguration configuration)
        {
            _systemAccountRepository = systemAccountRepository;
            _configuration = configuration;
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TokenResponse>> Login([FromBody] LoginRequest login)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _systemAccountRepository.GetAccountByEmailOrPassword(login.Email, login.Password);
                if (result == null)
                {
                    return BadRequest("Email or Password is incorrect");
                }
                var claims = new[]{
                    new Claim(ClaimTypes.Name,result.AccountName??""),
                    new Claim(ClaimTypes.Sid,result.AccountId.ToString()),
                    new Claim(ClaimTypes.Role,result.AccountRole.ToString()??"")
                };
                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:Key"] ?? new Exception("Key not found").ToString()));
                var signCredential= new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var jwtSecurityToken = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: signCredential
                );
                var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
                var tokenResponse = new TokenResponse
                {
                    Token = token,
                    AccountRole = result.AccountRole,
                    AccountId = result.AccountId,
                    AccountName = result.AccountName!,
                };
                return Ok(tokenResponse);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}