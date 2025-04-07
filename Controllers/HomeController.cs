using AutoMapper;
using Medicines.Data;
using Medicines.Data.dto;
using Medicines.Data.Models;
using Medicines.Services.Interfaces;
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
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (registerDto == null)
                return BadRequest(new { message = "Invalid request data" });

            var result = await _userService.RegisterAsync(registerDto);

            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message, token = result.Token });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            if (userDto == null || string.IsNullOrWhiteSpace(userDto.UserName) || string.IsNullOrWhiteSpace(userDto.Password))
                return BadRequest(new { message = "Name and password are required" });

            var result = await _userService.LoginAsync(userDto);

            if (result == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetUsersAsync();

            var result = users.Select(u => new
            {
                u.Id,
                u.Name,
                u.IsDleted,
                u.RoleId
            });

            return Ok(result);
        }
    }
}

