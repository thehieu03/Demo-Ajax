using System.IdentityModel.Tokens.Jwt;
using Entity.ModelResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = Entity.ModelRequest.LoginRequest;

namespace Server.API
{
    [Route("api/account")]
    [ApiController]
    public class SystemAccountController : ControllerBase
    {
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public SystemAccountController(ISystemAccountRepository systemAccountRepository, IConfiguration configuration,IMapper mapper)
        {
            _systemAccountRepository = systemAccountRepository;
            _configuration = configuration;
            _mapper=mapper;
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

                var key = new SymmetricSecurityKey(
                    System.Text.Encoding.UTF8.GetBytes(_configuration["JWT:Key"] 
                        ?? throw new Exception("Key not found"))
                );
                var signCredential = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var emailAccount = _configuration["AdminAccount:Email"] ?? throw new Exception("Email not found");
                var passwordAccount = _configuration["AdminAccount:Password"] ?? throw new Exception("Password not found");
                var roleAccount = _configuration["AdminAccount:Role"] ?? throw new Exception("Role not found");
                if (emailAccount.Equals(login.Email) && passwordAccount.Equals(login.Password))
                {
                    var claimsAdmin = new[]
                    {
                        new Claim(ClaimTypes.Name, "Admin"),
                        new Claim(ClaimTypes.Sid, "-1"),
                        new Claim(ClaimTypes.Role, roleAccount)
                    };

                    var jwtSecurityToken = new JwtSecurityToken(
                        issuer: _configuration["JWT:Issuer"],
                        audience: _configuration["JWT:Audience"],
                        claims: claimsAdmin,
                        expires: DateTime.Now.AddHours(3),
                        signingCredentials: signCredential
                    );

                    var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

                    return Ok(new TokenResponse
                    {
                        Token = token,
                        AccountRole = int.Parse(roleAccount),
                        AccountId = -1,
                        AccountName = "Admin",
                        CheckAdmin = true  
                    });
                }
                var result = await _systemAccountRepository.GetAccountByEmailOrPassword(login.Email, login.Password);
                if (result == null)
                {
                    return BadRequest("Email or Password is incorrect");
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, result.AccountName ?? ""),
                    new Claim(ClaimTypes.Sid, result.AccountId.ToString()),
                    new Claim(ClaimTypes.Role, result.AccountRole?.ToString() ?? "")
                };

                var jwtSecurityTokenUser = new JwtSecurityToken(
                    issuer: _configuration["JWT:Issuer"],
                    audience: _configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: signCredential
                );

                var tokenUser = new JwtSecurityTokenHandler().WriteToken(jwtSecurityTokenUser);

                return Ok(new TokenResponse
                {
                    Token = tokenUser,
                    AccountRole = result.AccountRole,
                    AccountId = result.AccountId,
                    AccountName = result.AccountName!,
                    CheckAdmin = result.AccountRole==2? true : false
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        public async Task<IActionResult> CreateAccount([FromBody]CreateAccount create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Input not found");
            }
             var account =_mapper.Map<SystemAccount>(create);
             await _systemAccountRepository.Create(account);
            return Ok("Create account successfully");
        }
        [HttpGet("getAllAccount")]
        public async Task<ActionResult<SystemAccountResponse>> GetAllAccount()
        {
            var accounts = await _systemAccountRepository.GetAllAsync();
            var response = _mapper.Map<IEnumerable<SystemAccountResponse>>(accounts);
            return Ok(response);
        }
    }
}
