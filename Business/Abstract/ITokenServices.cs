using Entities;
using Entities.ViewModels.TokenModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ITokenServices
    {
        string CreateTokenJWT(User user);
        Task<string> CreateTokenEmailConfirm(User user);
        Task<TokenValidation> ValidateToken(HttpContext context);
        string GenerateRefreshToken();
        Task SaveRefreshTokenAsync(User user, string refleshToken);
        Task<User> GetUserFromRefreshToken(string refreshToken);
    }
}
