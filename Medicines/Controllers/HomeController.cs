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
        public IActionResult Register([FromBody] RegisterDto registerDto)
        {
           
            if (context.Users.Any(u => u.PhoneNumber == registerDto.PhoneNumber))
            {
                return BadRequest(new { message = "User with this phone number already exists" });
            }

       
            var role = context.Roles.FirstOrDefault(r => r.Id == registerDto.RoleId);
            if (role == null)
            {
                return BadRequest(new { message = "Invalid RoleId" });
            }

        
            var newUser = new Users
            {
                Name = registerDto.Name,
                PhoneNumber = registerDto.PhoneNumber,
                RoleId = registerDto.RoleId,
                Role = role,
                IsDleted = false 
            };

            context.Users.Add(newUser);
            context.SaveChanges();

            var token = GenerateJwtToken(newUser);

     
            var userDto = _mapper.Map<UserDto>(newUser);

            return Ok(new { message = "User registered successfully", token, user = userDto });
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserDto UserDto) // ⬅️ استخدام `LoginDto` بدلاً من `Users`
        {
            var dbUser = context.Users
                .FirstOrDefault(u => u.Name == UserDto.Name && u.PhoneNumber == UserDto.PhoneNumber && !u.IsDleted);

            if (dbUser == null)
            {
                return Unauthorized(new { message = "Invalid name or phone number" });
            }

        
            var token = GenerateJwtToken(dbUser);

        
            var userDto = _mapper.Map<UserDto>(dbUser);

            return Ok(new { token, user = userDto });
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
