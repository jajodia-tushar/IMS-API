
using IMS.Entities;
using IMS.Entities.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IMS.TokenManagement
{
    public class JwtTokenProvider:ITokenProvider
    {
        private IConfiguration _configuration;
        public JwtTokenProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new[] {
                new Claim(ClaimTypes.Name,user.Id.ToString()),
                new Claim("Username",user.Username),
                new Claim(ClaimTypes.Role,user.Role.Name),
                 new Claim("Firstname",user.Firstname),
                 new Claim("Lastname",user.Lastname)
            };
            
            var token = new JwtSecurityToken(_configuration["Jwt:Issuer"],
               _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(60),
                signingCredentials: credentials);
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public int GetUserIdFromToken()
        {
            return -1;
        }
    }
}
