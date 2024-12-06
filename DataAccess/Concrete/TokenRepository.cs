using DataAccess.Abstract;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete
{
    public class TokenRepository : ITokenRepository
    {
        public async Task<IdentityUserToken<string>> GetUserTokenByRefreshTokenAsync(string refreshToken)
        {
            using (var _DBContext = new DBContext())
            {
                return await _DBContext.Set<IdentityUserToken<string>>()
                    .FirstOrDefaultAsync(t => t.Name == "RefreshToken" && t.Value == refreshToken);
            }
        }
    }
}
