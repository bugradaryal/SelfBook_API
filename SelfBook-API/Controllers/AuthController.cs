using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Entities;
using Business.Abstract;
using Business.Concrete;
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
using Entities.ViewModels.UserModels;
using Entities.ViewModels.ConfigurationModels;

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
        public async Task<IActionResult> RegisterUser([FromBody] RegisterUserModel registerModel)   
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
                return BadRequest(new {message = ex.Message});
            }
        }
        [HttpPost("LoginUser")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginUser([FromBody] LoginUserModel loginModel)        
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
                    var refleshToken = _tokenServices.GenerateRefreshToken();
                    await _tokenServices.SaveRefreshTokenAsync(user, refleshToken);

                    Response.Headers.Append("Authorization", "Bearer " + _tokenServices.CreateTokenJWT(user));
                    Response.Headers.Append("Refresh-Token", refleshToken);
                    return Ok(new { message = "Login Succesfull" });
                }
                else
                    return Unauthorized(new { message = "Email or password is wrong!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
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

                var emailConfUrl = await _tokenServices.CreateTokenEmailConfirm(user);          
                var callback_url = _jwt.Value.Audience + "/Auth/Emailverification?userId=" + user.Id + "&emailConfUrl=" + emailConfUrl;


                _emailService.SendingEmail(email, callback_url);
                return Ok(new { message = "Email verification code sended!!!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("Emailverification")]
        [AllowAnonymous]
        public async Task<IActionResult> Emailverification(string userId, string emailConfUrl)  
        {
            try
            {
                await _emailService.ConfirmEmail(userId, emailConfUrl);
                return Ok(new { message = "Your email has been successfully confirmed!" });
            }
            catch(Exception ex) 
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("DeleteUser")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(string password) 
        {
            try
            {
                var response = await _tokenServices.ValidateToken(HttpContext);
                if (response.errorMessage != null)
                    return BadRequest(new { message = response.errorMessage });
                await _userService.DeleteUser(response.user.Id, password);
                return Ok(new { message = "Your email has been successfully confirmed!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("UpdateUser")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(UpdateUserModel updateUser)
        {
            try
            {
                var response = await _tokenServices.ValidateToken(HttpContext);
                if (response.errorMessage != null) 
                    return BadRequest(new { message = response.errorMessage });
                if(!string.IsNullOrEmpty(updateUser.firstName))
                    response.user.FirstName = updateUser.firstName;

                if (!string.IsNullOrEmpty(updateUser.lastName))
                    response.user.LastName = updateUser.lastName;

                if (!string.IsNullOrEmpty(updateUser.UserName))
                    response.user.UserName = updateUser.UserName;

                if (!string.IsNullOrEmpty(updateUser.phoneNumber))
                    response.user.PhoneNumber = updateUser.phoneNumber;

                if (!string.IsNullOrEmpty(updateUser.Gender))
                    response.user.Gender = updateUser.Gender;

                await _userService.UpdateUser(response.user);
                Response.Headers.Append("Authorization", "Bearer " + _tokenServices.CreateTokenJWT(response.user));

                return Ok(new { message = "User is updated!!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

        }   
        [HttpPut("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword(string newPassword, string oldPassword) 
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
                return BadRequest(new { message = ex.Message });
            }

        }

        [HttpPost("RefreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var refleshToken = Request.Headers["Authorization"].FirstOrDefault();
                if (refleshToken == null)
                    return BadRequest(new { message = "RefreshToken not found." });
                var user = await _tokenServices.GetUserFromRefreshToken(refleshToken);
                if (user == null)
                    return BadRequest(new { message = "User not found." });
                var newRefreshToken = _tokenServices.GenerateRefreshToken();
                await _tokenServices.SaveRefreshTokenAsync(user, newRefreshToken);
                Response.Headers.Append("Authorization", "Bearer " + _tokenServices.CreateTokenJWT(user));
                Response.Headers.Append("Refresh-Token", newRefreshToken);

                return Ok(new { message = "Login Succesfull With RefreshToken" });
            }
            catch (Exception ex) 
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
