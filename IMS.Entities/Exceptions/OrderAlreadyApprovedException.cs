using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class OrderAlreadyApprovedException : Exception
    {
        public int ErrorCode = 400;
        public string ErrorMessage = "Order is Already Approved";
    }
}
