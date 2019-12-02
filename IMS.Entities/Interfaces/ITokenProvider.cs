
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Interfaces
{
    public interface ITokenProvider
    {
        string GenerateToken(User user);
        bool IsValidToken(string token);
        //int GetUserIdFromHeadersAuthorization(StringValues authorizationValues);
    }
}
