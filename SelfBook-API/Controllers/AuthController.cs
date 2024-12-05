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
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Http.Extensions;
using System.Text.Json.Nodes;
using static System.Net.Mime.MediaTypeNames;
using System.Security.Claims;
using Microsoft.AspNetCore.Http.HttpResults;

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
        private IOptions<JWT> _jwt;
        public AuthController(ILogger<AuthController> logger, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, SignInManager<User> signInManager, IOptions<EmailSender> emailSender)
        {
            _jwt = jwt;
            _logger = logger;         
            _userService = new UserManager(userManager, roleManager, jwt, signInManager);
            _tokenServices = new TokenManager(jwt,userManager);
            _emailService = new EmailManager(emailSender,userManager);
        }

        [HttpPost("RegisterUser")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterUser([FromBody] RegisterModel registerModel)   //+++
        {
            try
            {
                if(!ModelState.IsValid)
                    return BadRequest(new { message = ModelState });
                var response = await _userService.RegisterAsync(registerModel);
                return Ok(new { message = response });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("LoginUser")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser([FromBody] LoginModel loginModel)        //+++
        {
            try
            {  
                if (!ModelState.IsValid)
                    return BadRequest(new { message = ModelState });

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
                    Response.Headers.Append("Authorization", "Bearer " + _tokenServices.CreateTokenJWT(user));
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
        public async Task<IActionResult> SendingEmail(string email)     //+++
        {
            try 
            {
                var user = await _userService.GetUserByEmail(email);
                if (user == null)
                    return BadRequest(new { message = "Account does not exist!" });

                var emailConfUrl = await _tokenServices.CreateTokenEmailConfirm(user);          
                var callback_url = _jwt.Value.Audience + "/Auth/Emailverification?userId=" + user.Id + "&emailConfUrl=" + emailConfUrl;


                _emailService.SendingEmail(email, callback_url);
                return Ok(new { message = "Email verification code sended!!!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpGet("Emailverification")]
        [AllowAnonymous]
        public async Task<IActionResult> Emailverification(string userId, string emailConfUrl)  //+++
        {
            try
            {
                await _emailService.ConfirmEmail(userId, emailConfUrl);
            }
            catch(Exception ex) 
            {
                return BadRequest(ex);
            }
            return Ok(new { message = "Your email has been successfully confirmed!" });
        }
        [HttpPost("DeleteUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string userId, string password) //+++
        {
            try
            {
                await _userService.DeleteUser(userId, password);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
            return Ok(new { message = "Your email has been successfully confirmed!"});
        }
        [HttpPut("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UpdateUser updateUser)
        {
            try
            {
                var response = await _tokenServices.ValidateToken(HttpContext);
                if (response.errorMessage != null) 
                    return BadRequest(new { message = response.errorMessage });
                if(updateUser.firstName != string.Empty)
                    response.user.FirstName = updateUser.firstName;

                if (updateUser.lastName != string.Empty)
                    response.user.LastName = updateUser.lastName;

                if (updateUser.UserName != string.Empty)
                    response.user.UserName = updateUser.UserName;

                if (updateUser.phoneNumber != string.Empty)
                    response.user.PhoneNumber = updateUser.phoneNumber;

                await _userService.UpdateUser(response.user);
                Response.Headers.Append("Authorization", "Bearer " + _tokenServices.CreateTokenJWT(response.user));

                return Ok(new { message = "User is updated!!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }   //+++
        [HttpPut("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string newPassword, string oldPassword) //+++
        {
            try
            {
                var response = await _tokenServices.ValidateToken(HttpContext);
                if (response.errorMessage != null)
                    return BadRequest(new { message = response.errorMessage });

                var identityResult = await _userService.ChangePassword(response.user, oldPassword, newPassword);

                if (!identityResult.Succeeded)
                   return BadRequest(string.Join(", ", identityResult.Errors.Select(e => e.Description)));

                return Ok(new { message = "Password Changed!!" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

        }
    }
}
