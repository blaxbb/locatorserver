using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace LocatorServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        public UserApiController(SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        [HttpPost("auth")]
        public async Task<IActionResult> AuthAsync([FromBody] AuthModel model)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(model.UserName);
            if(user == null)
            {
                return BadRequest("Invalid login");
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, true, false);
            if (!result.Succeeded)
            {
                return BadRequest("Invalid login");
            }

            var token = new JwtSecurityTokenHandler();

            return Ok();

        }

        public class AuthModel
        {
            [Required]
            public string UserName { get; set; }
            [Required]
            public string Password { get; set; }
        }
    }
}
