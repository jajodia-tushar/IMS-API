using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class LoginResponse:Response
    {
        public string AccessToken { get; set; }
        public User User { get; set; }
    }
}
