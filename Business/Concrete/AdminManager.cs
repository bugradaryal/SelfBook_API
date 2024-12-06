using Business.Abstract;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AdminManager : IAdminService
    {
        private IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        public AdminManager(UserManager<User> userManager)
        {
            _userRepository = new UserRepository();
            _userManager = userManager;
        }

        public async Task<List<User>> ListAllUsers(int page, string orderBy)
        {
            var result = await _userRepository.GetAllUser(page, orderBy);
            return result;
        }
        public async Task DeleteUserAccount(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found!!");
            await _userManager.DeleteAsync(user);
        }
        public async Task GiveRoleUser(string userId, string Role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var user_oldRole = _userManager.GetRolesAsync(user).Result.FirstOrDefault();
            if(Role != user_oldRole)
            {
                var addResult = await _userManager.AddToRoleAsync(user, Role);
                if (addResult.Succeeded)
                {
                    var removeResult = await _userManager.RemoveFromRoleAsync(user, user_oldRole);
                }
                throw new Exception("Old Role cant be removed!");
            }
            throw new Exception("Role allready exist on user!");
        }
    }
}
