using DoAn.Data;
using DoAn.DTO;
using DoAn.Helpers;
using DoAn.Service;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace DoAn.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : Controller
    {

        private readonly IConfiguration _config;
        private readonly IUserService _userService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IEmailService _emailService;

        public AuthController(IConfiguration config, IUserService userService,IPasswordHasher<User> passwordHasher,IEmailService emailService)
        {
            _config = config;
            _userService = userService;
            _passwordHasher = passwordHasher;
            _emailService = emailService;   
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            User? userDb = null;

            if (!string.IsNullOrEmpty(dto.Provider))
            {
                if (dto.Provider == "GOOGLE")
                {
                    var payload = await GoogleJsonWebSignature.ValidateAsync(dto.IdTokenOrAccessToken, new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { _config["Google:ClientId"] }
                    });

                    userDb = await _userService.GetUserByEmail(payload.Email.ToLower());
                    if (userDb == null)
                    {
                        userDb = new User
                        {
                            id = Guid.NewGuid(),
                            Email = payload.Email.ToLower(),
                            Username = payload.Name.ToString(),
                            Gender = "Other",
                            RoleId = 1,
                            CreatedAt = DateTime.Now,
                            Address = "",
                            PhoneNumber = "",
                        };
                        await _userService.CreateUserAsync(userDb);
                           }
                }
                else if (dto.Provider == "FACEBOOK")
                {
                    var url = $"https://graph.facebook.com/me?fields=id,name,email&access_token={dto.IdTokenOrAccessToken}";
                    using var http = new HttpClient();
                    var response = await http.GetAsync(url);
                    if (!response.IsSuccessStatusCode)
                        return Unauthorized("Mã token Facebook không hợp lệ");

                    var content = await response.Content.ReadAsStringAsync();
                    var fbUser = JsonSerializer.Deserialize<FacebookUserInfo>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    userDb = await _userService.GetUserByEmail(fbUser.Email);
                    if (userDb == null)
                    {
                        userDb = new User
                        {
                            id = Guid.NewGuid(),
                            Email = fbUser.Email.ToString(),
                            Username = fbUser.Name.ToString(),
                            Gender = "Other",
                            RoleId = 1,
                            CreatedAt = DateTime.Now,
                            Address = "",
                            PhoneNumber = "",
                        };
                        await _userService.CreateUserAsync(userDb);
                    }
                }
                else return BadRequest("Nhà cung cấp không hợp lệ");
            }
            else
            {
                userDb = await _userService.GetUserByEmail(dto.Email);
                if(userDb == null)
                    return Unauthorized("Thông tin đăng nhập không hợp lệ");
                var result = _passwordHasher.VerifyHashedPassword(userDb, userDb.Password, dto.Password);
                if (result == PasswordVerificationResult.Failed)
                    return Unauthorized("Thông tin đăng nhập không hợp lệ");
            }
            // Tạo claims
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,userDb.id.ToString()),
                new Claim(ClaimTypes.Name,userDb.Username.ToString()),
                new Claim(JwtRegisteredClaimNames.Sub, dto.Email),
                new Claim(ClaimTypes.Role, userDb.RoleId == 2 ? "Admin": "User"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            var userDb = await _userService.GetUserByEmail(dto.Email);
            if (userDb?.Email == dto.Email || dto.Email == null){
                return BadRequest("Email không hợp lệ hoặc đã tồn tại");
            }

            var user = new User
            {
                id = Guid.NewGuid(),
                Username = dto.Username,
                Password = dto.Password, 
                Email = dto.Email,
                Gender = dto.Gender,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                RoleId = 1,
                CreatedAt = DateTime.Now
            };

            await _userService.CreateUserAsync(user);
            await _emailService.SendEmailAsync(dto.Email, "Đăng ký tài khoản thành công",user.Username);
            return Ok(new { message = "Đăng ký tài khoản thành công!" });
        }
        [HttpGet("check_email")]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            var userDb = await _userService.GetUserByEmail(email);
            return Ok(new { exists = userDb != null });
        }
    }
}  
