namespace Medicines.Services
{
    using AutoMapper;
    using Medicines.Data.dto;
    using Medicines.Data.Models;
    using Medicines.Repositories.Interfaces;
    using Medicines.Services.Interfaces;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public UserService(IUserRepository userRepository, IConfiguration config, IMapper mapper)
        {
            _userRepository = userRepository;
            _config = config;
            _mapper = mapper;
        }

        public async Task<(bool Success, string Message, string? Token)> RegisterAsync(RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Password) || string.IsNullOrWhiteSpace(dto.UserName))
                return (false, "Name and password are required", null);

            if (dto.Name == "string" || dto.Password == "string" || dto.UserName == "string"  )
                return (false, "Name and password are required", null);

            if (await _userRepository.IsUsernameTaken(dto.Name))
                return (false, "This username is already taken", null);

            var newUser = new Users
            {
                Name = dto.Name,
                LastName = dto.LastName,
                UserName = dto.UserName,
                Password = dto.Password,
                RoleId = 1,
                IsDleted = false
            };

            await _userRepository.AddUserAsync(newUser);

            var token = GenerateJwtToken(newUser);
            return (true, "User registered successfully", token);
        }

        public async Task<LoginDto?> LoginAsync(UserDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.UserName) || string.IsNullOrWhiteSpace(dto.Password))
                return null;

            var user = await _userRepository.GetUserWithPractitionerAsync(dto.UserName, dto.Password);
            if (user == null)
                return null;

            var token = GenerateJwtToken(user);

            var loginDto = new LoginDto
            {
                UserId = user.Id,
                RoleId = user.RoleId,
                Token = token
            };

            if (user.RoleId == 3 && user.Practitioner != null)
            {
                loginDto.PractitionerId = user.Practitioner.Id;
                loginDto.PractitionerName = user.Practitioner.NamePractitioner;

                if (user.Practitioner.Pharmacy != null)
                {
                    loginDto.Pharmacy = new
                    {
                        user.Practitioner.Pharmacy.Id,
                        user.Practitioner.Pharmacy.Name,
                        user.Practitioner.Pharmacy.Address,
                        
                    };
                }
            }

            return loginDto;
        }



        public async Task<List<Users>> GetUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        private string GenerateJwtToken(Users user)
        {
            var jwtConfig = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name ?? string.Empty) // تعديل هنا
    };

            if (!string.IsNullOrEmpty(user.Role?.Name))
                claims.Add(new Claim(ClaimTypes.Role, user.Role.Name!));

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
