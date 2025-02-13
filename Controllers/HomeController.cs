using AutoMapper;
using Medicines.Data;
using Medicines.Data.dto;
using Medicines.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Medicines.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IConfiguration configuration;
        private readonly IMapper _mapper;

        public HomeController(AppDbContext _context, IConfiguration _configuration ,IMapper mapper)
        {
            this.context = _context;
            this.configuration = _configuration;
            this._mapper = mapper;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null)
            {
                return BadRequest(new { message = "Invalid request data" });
            }

            // التحقق من صحة البيانات (يجب إدخال الاسم وكلمة المرور فقط)
            if (string.IsNullOrWhiteSpace(registerDto.Name) || string.IsNullOrWhiteSpace(registerDto.Password))
            {
                return BadRequest(new { message = "Name and password are required" });
            }

            try
            {
                // التحقق من عدم وجود نفس الاسم مسبقًا
                if (await context.Users.AnyAsync(u => u.Name == registerDto.Name))
                {
                    return BadRequest(new { message = "This username is already taken" });
                }

                // إنشاء المستخدم الجديد
                var newUser = new Users
                {
                    Name = registerDto.Name,
                    Password = registerDto.Password,
                    RoleId = 1, // ثابت: مستخدم عادي
                    IsDleted = false // الحفاظ على القيم الافتراضية
                };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();

                // إنشاء التوكن
                var token = GenerateJwtToken(newUser);

                // تحويل المستخدم إلى UserDto
                var userDto = _mapper.Map<UserDto>(newUser);

                return Ok(new { message = "User registered successfully", token});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }



        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            if (userDto == null || string.IsNullOrWhiteSpace(userDto.Name) || string.IsNullOrWhiteSpace(userDto.Password))
            {
                return BadRequest(new { message = "Name and password are required" });
            }

            var dbUser = await context.Users
                .FirstOrDefaultAsync(u => u.Name == userDto.Name && u.Password == userDto.Password && !u.IsDleted);

            if (dbUser == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            var token = GenerateJwtToken(dbUser);

            return Ok(new LoginDto
            {
                UserId = dbUser.Id,
                Token = token
            });
        }




        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await context.Users
                .Select(u => new
                {
               u.Id,
               u.Name,
               u.IsDleted,
               u.RoleId
                  
                })
                .ToListAsync();

            return Ok(users);
        }

        private string GenerateJwtToken(Users user)
        {
            var jwtConfig = configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
             
            };

            // إضافة الدور إذا كان متوفراً
            if (user.Role != null)
            {
                claims.Add(new Claim(ClaimTypes.Role, user.Role.Name));
            }

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


    }
}
