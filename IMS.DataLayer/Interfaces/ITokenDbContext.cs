using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
   public  interface ITokenDbContext
    {
        Task<bool> StoreToken(string accessToken, string hashToken, DateTime expirationTime);
        Task<bool> IsValidToken(string hashToken);
        Task<bool> DeleteToken(string hashToken);
    }
}
