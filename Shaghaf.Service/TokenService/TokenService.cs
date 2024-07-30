using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Shaghaf.Core.Entities.Identity;
using Shaghaf.Core.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Shaghaf.Service.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> CreateTokenAsync(AppUser user)
        {
            //Payload

            // 1. Private Calims

            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                new Claim(ClaimTypes.GivenName, user.UserName)
            };


            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(double.Parse( _configuration["JWT:DurationInDays"])),
                claims:authClaims,
                signingCredentials:new SigningCredentials(authKey,SecurityAlgorithms.HmacSha256Signature)
                
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}