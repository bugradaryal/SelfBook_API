﻿using Business.Abstract;
using Entities;
using Microsoft.AspNetCore.Identity;
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
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;
using System.Web;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using DataAccess.Abstract;
using DataAccess.Concrete;
using Entities.ViewModels.ConfigurationModels;
using Entities.ViewModels.TokenModels;

namespace Business.Concrete
{
    public class TokenManager : ITokenServices
    {
        private readonly JWT _jwt;
        private readonly SymmetricSecurityKey _key;
        private readonly UserManager<User> _userManager;
        private ITokenRepository _tokenRepository;
        public TokenManager(IOptions<JWT> jwt, UserManager<User> userManager) 
        {
            _jwt = jwt.Value;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            _userManager = userManager;
            _tokenRepository = new TokenRepository();
        }
        public string CreateTokenJWT(User user)
        {
            var userRole = _userManager.GetRolesAsync(user).Result.First();
            var claims = new []
            {
                new Claim(ClaimTypes.Role, userRole),
                new Claim("uid", user.Id),
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
        public async Task<string> CreateTokenEmailConfirm(User user) 
        { 
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string encodedToken = HttpUtility.UrlEncode(token);
            return encodedToken;
        }     
        public async Task<TokenValidation> ValidateToken(HttpContext context)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateIssuer = true,
                ValidateAudience = true,
                ClockSkew = TimeSpan.Zero,
                ValidateLifetime = true,
                ValidIssuer = _jwt.Issuer,
                ValidAudience = _jwt.Audience,
            };

            var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Bearer "))
                return new TokenValidation { errorMessage = "Token is nonvalid!!" };

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (!(validatedToken is JwtSecurityToken jwtToken))
                return new TokenValidation { errorMessage = "Token is nonvalid!!" };

            var userId = principal.FindFirst("uid")?.Value;

            var claimUser = await _userManager.FindByIdAsync(userId);

            if (claimUser == null)
                return new TokenValidation { errorMessage = "User not found!!" };

            var userRole = await _userManager.GetRolesAsync(claimUser);

            return new TokenValidation { user = claimUser, Role = userRole.ToList() };
        }
        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[32]; // 32 byte uzunluğunda bir byte dizisi
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(randomBytes); // Rastgele byte dizisi oluşturuluyor
            }
            return Convert.ToBase64String(randomBytes); // Base64 formatında döndürülür
        }
        public async Task SaveRefreshTokenAsync(User user, string refreshToken)
        {
            var result = await _userManager.SetAuthenticationTokenAsync(user, "MyProvider", "RefreshToken", refreshToken);
            if (!result.Succeeded)
            {
                throw new Exception("Refresh token save failed.");
            }
        }
        public async Task<User> GetUserFromRefreshToken(string refreshToken)
        {
            var userToken = await _tokenRepository.GetUserTokenByRefreshTokenAsync(refreshToken);
            if (userToken != null)
            {
                var user = await _userManager.FindByIdAsync(userToken.UserId);
                return user;
            }
            throw new Exception("Reflesh token corrupted!!");
        }
    }
}
