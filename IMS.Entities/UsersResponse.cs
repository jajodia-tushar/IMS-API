using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class UsersResponse : Response
    {
        public List<User> Users { get; set; }
    }
}
