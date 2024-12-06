using Business.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities;
using Entities.ViewModels.ConfigurationModels;
using Entities.ViewModels.UserModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly SignInManager<User> _signInManager;

        private IUserRepository _userRepository;
        public UserManager(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, SignInManager<User> signInManager)
        {
            _userRepository = new UserRepository();
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _signInManager = signInManager;
        }

        public async Task<string> RegisterAsync(RegisterUserModel model)        //response message alamıyoz düzelcek
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Gender = model.Gender,
                PhoneNumber = model.PhoneNumber,
            };
            var identityResult = await _userManager.CreateAsync(user, model.Password);
            if (identityResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                return $"User Registered {user.UserName}";
            }
            else
                return string.Join(", ", identityResult.Errors.Select(e => e.Description));
        }      
        public async Task<User> GetUserByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }
        public async Task<SignInResult> LoginAsync(User user, string password)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
            return result;
        }
        public async Task DeleteUser(string userid, string password)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if(user == null)
                throw new Exception("User not found!!");
            await _userManager.DeleteAsync(user);
        }
        public async Task UpdateUser(User user)
        {
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception("Cant update user!!    -    " + errorMessages);
            }
        }
        public async Task<IdentityResult> ChangePassword(User user, string oldPassword, string newPassword)
        {
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
            return result;
        }
    }
}
