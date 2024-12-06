using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstract
{
    public interface ITokenRepository
    {
        Task<IdentityUserToken<string>> GetUserTokenByRefreshTokenAsync(string refreshToken);
    }
}
