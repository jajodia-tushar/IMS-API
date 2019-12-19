using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class InvalidOrderException : CustomException
    {
        public InvalidOrderException()
        {
           ErrorCode = 400;
           ErrorMessage = "Invalid Order";
        }
    }
}
