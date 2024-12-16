using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GP.core.Entities.identity;
using GP.core.Services;
using Microsoft.AspNetCore.Http;

namespace GP.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
   

        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
        
        }
        public async Task<string> CreateTokenAsyn(AppUser user, UserManager<AppUser> userManager)
        {
            //private claims [user-defiend]

            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName,user.DisplayName),
                new Claim(ClaimTypes.Email,user.Email),
            };
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            //SECURITY KEY
            var authKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:Key"]));

            //register claims
            var token = new JwtSecurityToken(

                issuer: configuration["JWT:ValidIssuer"],
                audience: configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(double.Parse(configuration["JWT:DurationInDays"])),
                claims: authClaims,
                signingCredentials:new SigningCredentials(authKey,SecurityAlgorithms.HmacSha256Signature)
                );
            return new JwtSecurityTokenHandler().WriteToken(token);

        }
     
    }

}
