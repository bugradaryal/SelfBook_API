using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Entities;
using Business.Abstract;
using Business.Concrete;
using Entities.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using System.Reflection.PortableExecutable;

namespace SelfBook.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {

        private readonly ILogger<AuthController> _logger;
        private IUserService _userService;
        private ITokenServices _tokenServices;
        private IEmailService _emailService;
        public AuthController(ILogger<AuthController> logger, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, SignInManager<User> signInManager, IOptions<EmailSender> emailSender)
        {
            _logger = logger;         
            _userService = new UserManager(userManager, roleManager, jwt, signInManager);
            _tokenServices = new TokenManager(jwt);
            _emailService = new EmailManager(emailSender);
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
                    return BadRequest(new { message = "Account does not exist!" });
                var response = await _userService.LoginAsync(user,loginModel.Password);

                if(!user.EmailConfirmed)
                    return Unauthorized(new { message = "Require email configuration check your mailbox for configuration mail." });
                else if (response.IsLockedOut)
                    return Unauthorized(new { message = "Too many attempt. Account is locked for 5 min!" });
                else if (response.Succeeded)
                {
                    Response.Headers.Append("Authorization", "Bearer " + _tokenServices.CreateToken(user));
                    return Ok(new { message = "Login Succesfull" });
                }
                else
                    return Unauthorized(new { message = "Email or password is wrong!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("SendMail")]
        [AllowAnonymous]
        public async Task<IActionResult> SendingEmail(string email)
        {
            try 
            {
                var user = await _userService.GetUserByEmail(email);
                if (user == null)
                    return BadRequest(new { message = "Account does not exist!" });
                _emailService.SendingEmail(email);
                return Ok(new { message = "Email verification code sended!!!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
