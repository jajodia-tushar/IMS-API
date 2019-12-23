using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class InvalidOrderException : CustomException
    {
        public InvalidOrderException(string errorMessage= "Invalid Order")
        {
           ErrorCode = 400;
           ErrorMessage =errorMessage;
        }
    }
}
