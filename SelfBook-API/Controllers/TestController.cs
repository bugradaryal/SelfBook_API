using Business.Abstract;
using Business.Concrete;
using Entities.DTOs;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

namespace SelfBook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private ITokenServices _tokenServices;
        public TestController(UserManager<User> userManager, IOptions<JWT> jwt)
        {
            _tokenServices = new TokenManager(jwt, userManager);
        }
        [HttpPost("test1")]
        [AllowAnonymous]
        public async Task<IActionResult> test1()
        {
            return Ok(await _tokenServices.ValidateToken(HttpContext));
        }
    }
}
