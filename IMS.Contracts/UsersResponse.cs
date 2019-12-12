using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class UsersResponse : Response
    {
        public List<User> Users { get; set; }
    }
}
