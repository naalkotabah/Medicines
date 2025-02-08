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

            // التحقق من صحة البيانات
            if (string.IsNullOrWhiteSpace(registerDto.Name) ||
                string.IsNullOrWhiteSpace(registerDto.PhoneNumber) 
              )
            {
                return BadRequest(new { message = "All fields are required" });
            }

            try
            {
               
                if (await context.Users.AnyAsync(u => u.PhoneNumber == registerDto.PhoneNumber))
                {
                    return BadRequest(new { message = "هذا الرقم مستعمل بل قعل" });
                }

          

                // إنشاء المستخدم الجديد
                var newUser = new Users
                {
                    Name = registerDto.Name,
                    PhoneNumber = registerDto.PhoneNumber,
                    RoleId = 1, //User
                    IsDleted = false // يجب التأكد أن هذا الحقل لا يؤثر على الاستعلامات المستقبلية
                };

                context.Users.Add(newUser);
                await context.SaveChangesAsync();

                // إنشاء التوكن
                var token = GenerateJwtToken(newUser);

                // تحويل المستخدم إلى UserDto
                var userDto = _mapper.Map<UserDto>(newUser);

                return Ok(new { message = "User registered successfully", token, user = userDto });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request", error = ex.Message });
            }
        }


        [HttpPost("Login")]
        public async Task<LoginDto> Login([FromBody] UserDto UserDto)
        {
            var dbUser = context.Users
                .FirstOrDefault(u => u.Name == UserDto.Name && u.PhoneNumber == UserDto.PhoneNumber && !u.IsDleted);

            if (dbUser == null)
            {
                return null; 
            }

            var token = GenerateJwtToken(dbUser);

            return new LoginDto
            {
                UserId = dbUser.Id,
                Token = token
            };
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
                new Claim("PhoneNumber", user.PhoneNumber) // تخزين رقم الهاتف داخل التوكن
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
