using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IAdminService
    {
        Task<List<User>> ListAllUsers(int page, string orderBy);
        Task DeleteUserAccount(string userId);
        Task GiveRoleUser(string userId, string Role);
    }
}
