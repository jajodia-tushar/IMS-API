using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class OrderAlreadyApprovedException : CustomException
    {
        
        public OrderAlreadyApprovedException()
        {
            ErrorCode = 400;
            ErrorMessage = "Order is Already Approved";
        }
    }
}
