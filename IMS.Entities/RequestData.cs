using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class RequestData
    {
        public bool HasValidToken { get; set; }
        public User User { get; set; }
    }
}
