
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
        public async Task<string> GenerateToken(User user,DateTime expirationTime)
        {
            string key = Environment.GetEnvironmentVariable(_configuration["Jwt:Key"]);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
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
                expires: expirationTime,
                signingCredentials: credentials);
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<bool> IsValidToken(string accessToken)
        { bool isValid = false;
            try
            {
                if (!string.IsNullOrEmpty(accessToken))
                {

                    string hashToken = GetAccessTokenHashValue(accessToken);
                    isValid = await _tokenDbContext.IsValidToken(hashToken);
                }

            }
            catch(Exception e)
            {
                throw e;
            }
            return isValid;
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

        public async Task<bool> DeleteToken(string token)
        {
            try
            {

                string hashToken = GetAccessTokenHashValue(token);
                bool isTokenDeleted = await _tokenDbContext.DeleteToken(hashToken);
                if (!isTokenDeleted)
                    throw new Exception("Token Not Deleted");
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }

        }
    }
}
