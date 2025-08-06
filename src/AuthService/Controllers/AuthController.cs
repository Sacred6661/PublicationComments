using MassTransit;
using Messaging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using UserService.Data;
using UserService.Data.Models;
using UserService.Model;

namespace UserService.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController(UserManager<ApplicationUser> userManager, IConfiguration config, RoleManager<IdentityRole> roleManager, 
        ApplicationDbContext dbContext, IPublishEndpoint publish) : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IConfiguration _config = config;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IPublishEndpoint _publish = publish;

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // basic role - user
                string roleName = "User";

                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    await _roleManager.CreateAsync(new IdentityRole(roleName));
                }

                await _userManager.AddToRoleAsync(user, roleName);
            }

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            var userAspNet = new UserRegistered
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            await _publish.Publish(userAspNet);


            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                await LoginLog(user?.Id, false);
                return Unauthorized("Invalid credentials");
            }

            await LoginLog(user.Id, true);

            var token = await GenerateJwtToken(user);
            return Ok(new { token });
        }

        private async Task<string> GenerateJwtToken(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim("UserId", user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
            };

            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var audiences = _config.GetSection("Jwt:Audiences").Get<string[]>();
            claims.AddRange(audiences.Select(aud => new Claim(JwtRegisteredClaimNames.Aud, aud)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["Jwt:ExpiresInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task LoginLog(string userId, bool isSuccess)
        {
            var log = new UserLoginLog
            {
                UserId = userId,
                LoginTime = DateTime.UtcNow,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString(),
                UserAgent = HttpContext.Request.Headers["User-Agent"].ToString(),
                IsSuccess = isSuccess
            };

            _dbContext.UserLoginLogs.Add(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}
