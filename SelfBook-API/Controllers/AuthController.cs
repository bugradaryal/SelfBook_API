using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Entities;
using Business.Abstract;
using Business.Concrete;
using Entities.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace SelfBook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<AuthController> _logger;
        private IUserService _userService;
        private ITokenServices _tokenServices;

        public AuthController(ILogger<AuthController> logger, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, SignInManager<User> signInManager)
        {
            _logger = logger;         
            _userService = new UserManager(userManager, roleManager, jwt, signInManager);
            _tokenServices = new TokenManager(jwt);
        }

        [HttpGet("Authorize")]
        [Authorize]
        public async Task<IActionResult> Authorize()
        {
            return Ok();
        }

        [HttpGet("AllowAnonymous")]
        [AllowAnonymous]
        public async Task<IActionResult> AllowAnonymous()
        {
            return Ok();
        }




        [HttpPost("RegisterUser")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel registerModel)
        {
            try
            {
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);
                var response = await _userService.RegisterAsync(registerModel);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("LoginUser")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel loginModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userService.GetUserByEmail(loginModel.Email);
                if (user == null)
                    return Unauthorized("Account does not exist!");
                var response = await _userService.LoginAsync(user,loginModel.Password);
                if (response.IsLockedOut)
                    return BadRequest("Too many attempt. Account is locked for 5 min!");
                else if(response.RequiresTwoFactor)
                    return BadRequest("Require email configuration check your mailbox for two-factor configuration mail.");
                else if (response.Succeeded)
                {
                    return Ok(new AuthenticationModel
                    {
                        Email = user.Email,
                        UserName = user.UserName,
                        Token = _tokenServices.CreateToken(user)
                    });
                }
                else
                    return BadRequest("Email or password is wrong!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
