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

namespace Server.API
{
    [Route("api/account")]
    [ApiController]
    public class SystemAccountController : ControllerBase
    {
        private readonly ISystemAccountRepository _systemAccountRepository;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public SystemAccountController(ISystemAccountRepository systemAccountRepository, IConfiguration configuration, IMapper mapper)
        {
            _systemAccountRepository = systemAccountRepository;
            _configuration = configuration;
            _mapper = mapper;
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
        [HttpPost("createAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAccount([FromBody] CreateAccount create)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Log thông tin đầu vào
                Console.WriteLine($"[CREATE_ACCOUNT] Bắt đầu tạo tài khoản với email: {create.AccountEmail}");
                
                // Kiểm tra email đã tồn tại chưa
                if (string.IsNullOrEmpty(create.AccountEmail))
                {
                    Console.WriteLine("[CREATE_ACCOUNT] Email trống");
                    return BadRequest("Email không được để trống");
                }
                
                var existingAccount = await _systemAccountRepository.GetByEmail(create.AccountEmail);
                if (existingAccount != null)
                {
                    Console.WriteLine($"[CREATE_ACCOUNT] Email {create.AccountEmail} đã tồn tại");
                    return BadRequest("Email đã được sử dụng. Vui lòng chọn email khác.");
                }

                var account = _mapper.Map<SystemAccount>(create);
                account.AccountRole = 1; // Role 1 = User thường
                
                // Log thông tin account trước khi tạo
                Console.WriteLine($"[CREATE_ACCOUNT] Account mapping thành công:");
                Console.WriteLine($"  - AccountId: {account.AccountId}");
                Console.WriteLine($"  - AccountName: {account.AccountName}");
                Console.WriteLine($"  - AccountEmail: {account.AccountEmail}");
                Console.WriteLine($"  - AccountRole: {account.AccountRole}");
                Console.WriteLine($"  - AccountPassword: {account.AccountPassword}");
                
                // Đảm bảo AccountId = 0 để DAO tự động tạo ID
                account.AccountId = 0;
                Console.WriteLine($"[CREATE_ACCOUNT] Đã set AccountId = 0 để tự động tạo ID");
                
                await _systemAccountRepository.Create(account);
                
                Console.WriteLine($"[CREATE_ACCOUNT] Tạo tài khoản thành công cho email: {create.AccountEmail}");
                return Ok("Tài khoản đã được tạo thành công!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CREATE_ACCOUNT] Lỗi khi tạo tài khoản: {ex.Message}");
                Console.WriteLine($"[CREATE_ACCOUNT] Stack trace: {ex.StackTrace}");
                
                // Log inner exception nếu có
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"[CREATE_ACCOUNT] Inner exception: {ex.InnerException.Message}");
                    Console.WriteLine($"[CREATE_ACCOUNT] Inner stack trace: {ex.InnerException.StackTrace}");
                }
                
                return BadRequest($"Lỗi khi tạo tài khoản: {ex.Message}");
            }
        }
        [HttpGet("getAllAccount")]
        public async Task<ActionResult<SystemAccountResponse>> GetAllAccount()
        {
            var accounts = await _systemAccountRepository.GetAllAsync();
            var response = _mapper.Map<IEnumerable<SystemAccountResponse>>(accounts);
            return Ok(response);
        }
        [HttpGet("getAccountByEmail")]
        public async Task<ActionResult<SystemAccountResponse>> GetAccountByEmail([FromQuery] string email)
        {
            var account = await _systemAccountRepository.GetByEmail(email);
            if (account == null)
            {
                return NotFound("Account not found");
            }
            var response = _mapper.Map<SystemAccountResponse>(account);
            return Ok(response);
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
        [Authorize]
        [HttpPut("updateAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<SystemAccountUserResponse>> UpdateAccount([FromBody] SystemAccountUserResponse model)
        {
            var account= await _systemAccountRepository.GetById(model.AccountId);
            if (account == null)
            {
                return BadRequest("Account not found");
            }
            if (account.AccountId != User.GetAccountId())
            {
                return Unauthorized("You are not authorized to update this account");
            }
            account.AccountName = model.AccountName;
            account.AccountEmail = model.AccountEmail;
            account.AccountPassword = model.AccountPassword;
            await _systemAccountRepository.Update(account);
            var response = _mapper.Map<SystemAccountUserResponse>(account);
            return Ok(response);
        }

    }
}
