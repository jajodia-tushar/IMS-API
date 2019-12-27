using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class RolesResponse : Response
    {
        public List<Role> Roles { get; set; }
    }
}
