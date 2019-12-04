
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface ILoginService
    {
        Task<LoginResponse> Login(LoginRequest loginRequest);
       
        
    }
}
