using System.IdentityModel.Tokens.Jwt;
using Entity.ModelResponse;
using Entity.ModelRequest;
using Entity.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity.Data;
using LoginRequest = Entity.ModelRequest.LoginRequest;
using Microsoft.AspNetCore.Authorization;
using Server.Services;

namespace Server.API
{
    [Route("api/account")]
    [ApiController]
    public class SystemAccountController : ControllerBase
    {
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;

        public SystemAccountController(ISystemAccountRepository systemAccountRepository, IConfiguration configuration, IMapper mapper, IEmailService emailService)
        {
            _systemAccountRepository = systemAccountRepository;
            _configuration = configuration;
            _mapper = mapper;
            _emailService = emailService;
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
                    CheckAdmin = result.AccountRole == 2 ? true : false
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("getAllAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<SystemAccountResponse>>> GetAllAccount()
        {
            try
            {
                var result = await _systemAccountRepository.GetAllAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getAccountByEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SystemAccount>> GetAccountByEmail([FromQuery] string email)
        {
            try
            {
                var account = await _systemAccountRepository.GetByEmail(email);
                if (account == null)
                {
                    return NotFound();
                }
                return Ok(account);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("createAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SystemAccount>> CreateAccount([FromBody] CreateAccount create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (string.IsNullOrEmpty(create.AccountEmail))
                {
                    return BadRequest("Email không được để trống");
                }
                
                var existingAccount = await _systemAccountRepository.GetByEmail(create.AccountEmail);
                if (existingAccount != null)
                {
                    return BadRequest("Email đã được sử dụng. Vui lòng chọn email khác.");
                }

                var account = _mapper.Map<SystemAccount>(create);
                account.AccountRole = 1; 
                account.AccountId = 0;
                
                await _systemAccountRepository.Create(account);
                return Ok("Tài khoản đã được tạo thành công!");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                }
 
                return BadRequest($"Lỗi khi tạo tài khoản: {ex.Message}");
            }
        }
        [Authorize]
        [HttpGet("getAccountById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SystemAccountResponse>> GetAccountById()
        {
            var accountId = User.GetAccountId();
            if (accountId < 0)
            {
                return Unauthorized("Invalid account");
            }

            var account = await _systemAccountRepository.GetById(accountId);
            if (account == null)
            {
                return NotFound("Account not found");
            }

            var response = _mapper.Map<SystemAccountUserResponse>(account);
            return Ok(response);
        }

        [HttpPost("forgotPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ForgotPassword([FromBody] Entity.ModelRequest.ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var existingAccount = await _systemAccountRepository.GetByEmail(request.Email);
                if (existingAccount == null)
                {
                    return NotFound("Email không tồn tại trong hệ thống");
                }


                const string newPassword = "123456";
                

                existingAccount.AccountPassword = newPassword;
                await _systemAccountRepository.Update(existingAccount);


                var emailSent = await _emailService.SendPasswordResetEmailAsync(request.Email, newPassword);
                
                if (emailSent)
                {
                    return Ok(new { message = "Mật khẩu mới đã được gửi đến email của bạn" });
                }
                else
                {
                    return BadRequest("Không thể gửi email. Vui lòng thử lại sau");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra: {ex.Message}");
            }
        }
    }
}