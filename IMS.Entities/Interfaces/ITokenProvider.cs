
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface ITokenProvider
    {
        Task<string> GenerateToken(User user);
        Task<bool> IsValidToken(string token);
        Task<bool> StoreToken(string token,User user);
        //int GetUserIdFromHeadersAuthorization(StringValues authorizationValues);
    }
}
