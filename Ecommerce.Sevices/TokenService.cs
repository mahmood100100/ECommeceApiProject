using Ecommerce.Core.Entities;
using Ecommerce.Core.IRepositories.IServices;
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

namespace Ecommerce.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration configuration;
        private readonly UserManager<LocalUser> userManager;
        private readonly string secretKey;

        public TokenService(IConfiguration configuration , UserManager<LocalUser> userManager)
        {
            this.configuration = configuration;
            this.userManager = userManager;
            secretKey = configuration.GetSection("ApiSettings")["SecretKey"];
        }
        public async Task<string> CreateTokenAsync(LocalUser user)
        {
            var key = Encoding.ASCII.GetBytes(secretKey);
            var roles = await userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name , user.UserName),
                new Claim (ClaimTypes.GivenName , $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Email , user.Email),
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role , role)));
            var TokenDesciptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key) , SecurityAlgorithms.HmacSha256Signature),

            };

            var TokenHandler = new JwtSecurityTokenHandler();
            return TokenHandler.WriteToken(TokenHandler.CreateToken(TokenDesciptor));
        }
    }
}
