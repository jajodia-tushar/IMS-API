
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Interfaces
{
    public interface ILoginService
    {
        Response Login(LoginRequest loginRequest);

    }
}
