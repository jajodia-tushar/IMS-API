using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class LoginResponse:Response
    {
        public string AccessToken { get; set; }
        public User User { get; set; }
    }
}
