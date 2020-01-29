using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ChangePasswordDetails
    {
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
}
