using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class InvalidEmailException:CustomException
    {
        public InvalidEmailException(string errorMessage = "Invalid Email")
        {
            ErrorCode = 400;
            ErrorMessage = errorMessage;
        }
    }
}
