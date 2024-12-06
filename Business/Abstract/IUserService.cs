using Entities;
using Entities.ViewModels.UserModels;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IUserService
    {
        Task<string> RegisterAsync(RegisterUserModel model);
        Task<User> GetUserByEmail(string email);
        Task<SignInResult> LoginAsync(User user, string password);
        Task DeleteUser(string userid, string password);
        Task UpdateUser(User user);
        Task<IdentityResult> ChangePassword(User user, string oldPassword, string newPassword);
    }
}
