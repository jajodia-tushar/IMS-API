using IMS.Entities;
using IMS.Entities.Exceptions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace IMS.Core
{
    public static class Utility
    {
        public static User GetUserFromToken(string accessToken)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(accessToken);
                var tokenS = handler.ReadToken(accessToken) as JwtSecurityToken;

                var userId = Int32.Parse(tokenS.Claims.First(claim => claim.Type == "UserId").Value);
                var username = tokenS.Claims.First(claim => claim.Type == "Username").Value;
                var firstname = tokenS.Claims.First(claim => claim.Type == "Firstname").Value;
                var lastname = tokenS.Claims.First(claim => claim.Type == "Lastname").Value;
                var roleId = Int32.Parse(tokenS.Claims.First(claim => claim.Type == "RoleId").Value);
                var roleName = tokenS.Claims.First(claim => claim.Type == ClaimTypes.Role).Value;
                return new User()
                {
                    Id = userId,
                    Username = username,
                    Firstname = firstname,
                    Lastname = lastname,
                    Role = new Role()
                    {
                        Id = roleId,
                        Name = roleName
                    }

                };
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Error ErrorGenerator(int errorCode, string errorMessage)
        {
            return new Error()
            {
                ErrorCode = errorCode,
                ErrorMessage = errorMessage
            };
        }

        public static string Hash(string text)
        {
            string returnValue = null;
            if (string.IsNullOrEmpty(text.Trim()))
                throw new CustomException();


            if (!String.IsNullOrEmpty(text))
            {
                using (MD5 md5Hash = MD5.Create())
                {
                    byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(text));
                    returnValue = Convert.ToBase64String(data);
                }
            }



            return Convert.ToBase64String(Encoding.Unicode.GetBytes(returnValue));
        }
    }
}
