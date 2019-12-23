using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class InvalidTokenException:CustomException
    {
       
        public InvalidTokenException(string errorMessage= "Invalid Token")
        {
            ErrorCode = 400;
            ErrorMessage =errorMessage;
        }
    }
}
