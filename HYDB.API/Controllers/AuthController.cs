using HYDB.Services.DTO;
using HYDB.Services.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HYDB.API.Controllers
{
    [Route("auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserAccountInfo _userAccountInfoService;

        public AuthController(IUserAccountInfo userAccountInfoService)
        {
            _userAccountInfoService = userAccountInfoService;
        }

        [HttpPost]
        [Route("createaccount")]
        public IActionResult CreateAccount(UserAccountPayload newUserAccountDetails)
        {
            return Ok(_userAccountInfoService.CreateAccount(newUserAccountDetails));
        }

        [Route("token/get")]
        [HttpPost]
        public IActionResult GenerateToken(UserAccountPayload newTokenRequest)
        {
            var authentication = _userAccountInfoService.AuthenticateUser(newTokenRequest);
            if (authentication.IsSuccess)
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes("PDv7DrqznYL6nv7DrqzjnQYO9JxIsWdcjnQYL6nu0f");
                var signinKey = new SymmetricSecurityKey(key);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Issuer = "hydb.xyz",
                    Audience = "hydb.xyz",
                    SigningCredentials = new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256),
                    Subject = new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.Email, newTokenRequest.UserName)
                    }),

                    Expires = DateTime.UtcNow.AddDays(15)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                return Ok(new { Token = tokenHandler.WriteToken(token) });
            }
            else
            {
                return Unauthorized(authentication.Message);
            }
        }

        [Route("token/verify")]
        [HttpGet]
        public IActionResult ValidateToken()
        {
            if ((HttpContext.User.Identity.IsAuthenticated))
            {
                return Ok(new { isValid = true });
            }
            else
            {
                return Ok(new { isValid = false });
            }
        }
    }
}
