using Business.Abstract;
using Entities;
using Entities.DTOs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class TokenManager : ITokenServices
    {
        private readonly Entities.DTOs.JWT _jwt;
        private readonly SymmetricSecurityKey _key;
        public TokenManager(IOptions<JWT> jwt) 
        {
            _jwt = jwt.Value;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        }
        public string CreateToken(User user)
        {
            var claims = new []
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("uid", user.Id)
            };
            var signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature);

            var tokenDescrtiptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwt.DurationInMinutes),
                SigningCredentials = signingCredentials,
                Issuer = _jwt.Issuer,
                Audience = _jwt.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescrtiptor); //hata

            return tokenHandler.WriteToken(token);
        }
    }
}
