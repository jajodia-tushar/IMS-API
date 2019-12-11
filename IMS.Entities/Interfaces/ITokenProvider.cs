
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface ITokenProvider
    {
        string GenerateToken(User user, DateTime expirationTime);
        Task<bool> IsValidToken(string token);
        Task<bool> StoreToken(string token,DateTime expirationTime);

        Task<bool> DeleteToken(string token);
    }
}
