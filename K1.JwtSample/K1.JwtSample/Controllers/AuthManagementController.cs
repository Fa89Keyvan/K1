using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using K1.JwtSample.Configuration;
using K1.JwtSample.Models.Dto.Requests;
using K1.JwtSample.Models.Dto.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace K1.JwtSample.Controllers
{
    [Route("api/[controller]")] //api/AuthManagement
    public class AuthManagementController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtConfig _jwtConfig;
        
        public AuthManagementController(UserManager<IdentityUser> userManager, IOptionsMonitor<JwtConfig> optionsMonitor)
        {
            _userManager = userManager;
            _jwtConfig = optionsMonitor.CurrentValue;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegistrationResponse
                {
                    Success = false,
                    Errors = new List<string>
                    {
                        "Invalid payload",
                    }
                });
            }

            IdentityUser existingUser = await _userManager.FindByEmailAsync(user.Email);
            if (existingUser != null)
            {
                return BadRequest(new RegistrationResponse
                {
                    Success = false,
                    Errors = new List<string> { "Email already exists" }
                });
            }

            var newUser = new IdentityUser(user.UserName);
            IdentityResult isCreatedUser = await _userManager.CreateAsync(newUser, user.Password);
            if (!isCreatedUser.Succeeded)
            {
                return BadRequest(new RegistrationResponse
                {
                    Errors = isCreatedUser.Errors.Select(x => x.Description).ToList()
                });
            }
            
            
        }

        private string GenerateJwtToken(IdentityUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new []
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email,user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub,user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString())
                })
            };
        }
    }
}