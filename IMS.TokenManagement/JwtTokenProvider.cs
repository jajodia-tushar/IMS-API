
using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace IMS.TokenManagement
{
    public class JwtTokenProvider:ITokenProvider
    {
        private IConfiguration _configuration;
        private ITokenDbContext _tokenDbContext;
        public JwtTokenProvider(IConfiguration configuration,ITokenDbContext tokenDbContext)
        {
            _configuration = configuration;
            _tokenDbContext = tokenDbContext;
        }
        public async Task<string> GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            
            var claims = new[] {
                new Claim("UserId",user.Id.ToString()),
                new Claim("Username",user.Username),
                new Claim(ClaimTypes.Role,user.Role.Name),
                new Claim("RoleId",user.Role.Id.ToString()),
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

        public async Task<bool> IsValidToken(string token)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> StoreToken(string accessToken,DateTime expirationTime)
        {
            try
            {
               
                string hashToken = GetAccessTokenHashValue(accessToken);
                bool isTokenStored = await _tokenDbContext.StoreToken(accessToken, hashToken, expirationTime);
                 if (!isTokenStored)
                    throw new Exception("Token Not Stored");
                return true;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        public static string GetAccessTokenHashValue(string accessTokenJwtString)
        {
            string returnValue = null;



            if (!String.IsNullOrEmpty(accessTokenJwtString))
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(accessTokenJwtString));
                    returnValue = Convert.ToBase64String(data);
                }
            }



            return Convert.ToBase64String(Encoding.Unicode.GetBytes(returnValue));
        }
       

       
    }
}
