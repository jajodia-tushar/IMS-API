using IMS.Entities;
using IMS.Entities.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using IMS.TokenManagement;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json.Linq;



namespace IMS.UnitTest.CoreTests
{
    public class JWTTokenProviderTests
    {
        public IConfiguration _configuration;
        public JWTTokenProviderTests()
        {
            _configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();
        }

        [Fact]
        public void Get_String_Type_JWT_Token_Succeeds()
        {
            var user = GetUserDetails();
            JwtTokenProvider jwtTokenprovider = new JwtTokenProvider(_configuration);
            var response = jwtTokenprovider.GenerateToken(user);
            Assert.IsType<String>(response);
        }

        [Fact]
        public void Returns_Success_when_Jwt_Token_Is_Not_Null()
        {
            var user = GetUserDetails();
            JwtTokenProvider jwtTokenprovider = new JwtTokenProvider(_configuration);
            var response = jwtTokenprovider.GenerateToken(user);
            Assert.NotNull(response);
        }

        [Fact]
        public void Returns_Success_When_Check_For_FirstName_And_Lastname_From_JWT_Token_Claims()
        {
            var user = GetUserDetails();
            JwtTokenProvider jwtTokenprovider = new JwtTokenProvider(_configuration);
            var response = jwtTokenprovider.GenerateToken(user);
            string[] words = response.Split('.');
            string payload = words[1];
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
            var payloadData = JObject.Parse(payloadJson);
            Assert.Equal("rochit", JObject.Parse(payloadData.ToString())["Firstname"]);
            Assert.Equal("aggarwal", JObject.Parse(payloadData.ToString())["Lastname"]);
        }

        private static byte[] Base64UrlDecode(string input)
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

        public User GetUserDetails()
        {
            return new User()
            {
                Id = 1,
                Username = "admin",
                Password = null,
                Firstname = "rochit",
                Lastname = "aggarwal",
                Email = "rochitaggarwal54@gmail.com",
                Role = new Entities.Role()
                {
                    Id = 1,
                    Name = "admin"
                }
            };
        }
    }
}