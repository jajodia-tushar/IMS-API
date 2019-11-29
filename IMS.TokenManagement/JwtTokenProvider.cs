
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
                new Claim("UserId",user.Id.ToString()),
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

        public int GetUserIdFromHeadersAurhorization(StringValues authorizationValues)
        {
            string accessToken = GetTokenFromStringValues(authorizationValues);
            if (accessToken == null)
                return -1;
            string payload = accessToken.Split(".")[1];
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecodePayload(payload));
            var payloadData = JObject.Parse(payloadJson);
            string UserId=(string)JObject.Parse(payloadData.ToString())["UserId"];
            int id = Int32.Parse(UserId);
            return id;
            
        }

        public int GetUserIdFromHeadersAuthorization(StringValues authorizationValues)
        {
            throw new NotImplementedException();
        }

        private  byte[] Base64UrlDecodePayload(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default: throw new System.Exception("Illegal base64url string!");
            }
            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }

        private string GetTokenFromStringValues(StringValues authenticateToken)
        {
            if (authenticateToken == StringValues.Empty)
                return null;
            string[] authTokenParts = authenticateToken.ToString().Split(" ");
            string bearer = authTokenParts[0];
            string token = authTokenParts[1];
            if (!bearer.Equals("Bearer") || token == null)
                return null;
            return token;
        }
    }
}
