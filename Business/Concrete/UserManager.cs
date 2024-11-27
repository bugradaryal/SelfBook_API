using Business.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities;
using Entities.DTOs;
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
        private readonly Entities.DTOs.JWT _jwt;
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

        public async Task<string> RegisterAsync(RegisterModel model)        //response message alamıyoz düzelcek
        {
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };
            var ıdentityResult = await _userManager.CreateAsync(user, model.Password);
            if (ıdentityResult.Succeeded)
            {
                _userManager.AddToRoleAsync(user, Authorization.default_role.ToString());
                return $"User Registered {user.UserName}";
            }
            else
                return $"Email or Username is already exist.";
        }
        
        public async Task<User> GetUserByEmail (string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<SignInResult> LoginAsync(User user, string password)
        {
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);
            return result;
        }









        public async Task<string> AddRoleAsync(AddRoleModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return $"No Accounts Registered with {model.Email}.";
            }
            if (await _userManager.CheckPasswordAsync(user, model.Password))
            {
                var roleExists = Enum.GetNames(typeof(Authorization.Roles))
                    .Any(x => x.ToLower() == model.Role.ToLower());
                if (roleExists)
                {
                    var validRole = Enum.GetValues(typeof(Authorization.Roles)).Cast<Authorization.Roles>()
                        .Where(x => x.ToString().ToLower() == model.Role.ToLower())
                        .FirstOrDefault();

                    await _userManager.AddToRoleAsync(user, validRole.ToString());
                    return $"Added {model.Role} to user {model.Email}.";
                }
                return $"Role {model.Role} not found.";
            }
            return $"Incorrect Credentials for user {user.Email}.";
        }
    }
}
