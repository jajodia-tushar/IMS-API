using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class InvalidTokenException:CustomException
    {
       
        public InvalidTokenException()
        {
            ErrorCode = 400;
            ErrorMessage = "Invalid Token";
        }
    }
}
