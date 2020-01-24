using IMS.Entities;
using IMS.Entities.Exceptions;
using IMS.Entities.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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

        public static String GenerateKey(Object sourceObject)
        {
            String hashString;
            if (sourceObject == null)
            {
                throw new ArgumentNullException("Null as parameter is not allowed");
            }
            else
            {
                try
                {
                    hashString = ComputeHash(ObjectToByteArray(sourceObject));
                    return hashString;
                }
                catch (AmbiguousMatchException ame)
                {
                    throw new ApplicationException("Could not definitely decide if object is serializable.Message:"+ame.Message);
                }
            }
        }

        private static string ComputeHash(byte[] objectAsBytes)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            try
            {
                byte[] result = md5.ComputeHash(objectAsBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < result.Length; i++)
                {
                    sb.Append(result[i].ToString("X2"));
                }
                return sb.ToString();
            }
            catch (ArgumentNullException ane)
            {
                return null;
            }
        }

        private static readonly Object locker = new Object();

        private static byte[] ObjectToByteArray(Object objectToSerialize)
        {
            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                lock (locker)
                {
                    formatter.Serialize(fs, objectToSerialize);
                }
                return fs.ToArray();
            }
            catch (SerializationException se)
            {
                return null;
            }
            finally
            {
                fs.Close();
            }
        }

        public static async Task<RequestData> GetRequestDataFromHeader(IHttpContextAccessor _httpContextAccessor, ITokenProvider _tokenProvider)
        {
            RequestData requestData = new RequestData
            {
                HasValidToken = false
            };

            try
            {
                string token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Split(" ")[1];
                bool isValidToken = await _tokenProvider.IsValidToken(token);
                if (isValidToken)
                {
                    requestData.User = Utility.GetUserFromToken(token);
                    requestData.HasValidToken = true;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                requestData.HasValidToken = false;
            }
            catch (Exception e)
            {
                throw e;
            }
            return requestData;
        }
    }
}
