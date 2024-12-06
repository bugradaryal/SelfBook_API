using Business.Abstract;
using Business.Concrete;
using Entities;
using Entities.ViewModels.ConfigurationModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Data;

namespace SelfBook.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private IAdminService _adminService;
        private ITokenServices _tokenServices;
        public AdminController(IOptions<JWT> jwt, UserManager<User> userManager)
        {
            _adminService = new AdminManager(userManager);
            _tokenServices = new TokenManager(jwt,userManager);
        }



        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GiveRoleToUser(string userId, string Role = "User")
        {
            try
            {
                var response = await _tokenServices.ValidateToken(HttpContext);
                if (response.errorMessage != null)
                    return BadRequest(new { message = response.errorMessage });
                if (Role != "User" || Role != "Moderator" || Role != "Administrator")
                    return BadRequest(new { message = "Undefined Role!!" });
                if (response.user.Id == userId)
                    return BadRequest(new { message = "You cant change your own role!!" });
                await _adminService.GiveRoleUser(userId, Role);
                return Ok(new { message = "Role added to the user!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Moderator, Administrator")]
        public async Task<IActionResult> ListAllUsers(int page = 1, string orderBy = "name")
        {
            try
            {
                if (orderBy != "FirstName" || orderBy != "LastName" || orderBy != "Gender" || orderBy != "UserName" || orderBy != "Email" || orderBy != "PhoneNumber")
                    return BadRequest(new { message = "Wrong order type!!" });
                var result = await _adminService.ListAllUsers(page, orderBy);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteUserAccount(string userId) //ban
        {
            try
            {
                await _adminService.DeleteUserAccount(userId);
                return Ok(new { message = "Account Deleted!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
