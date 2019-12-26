using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class InvalidUserNameException : CustomException
    {
        public InvalidUserNameException(string errorMessage = "Invalid UserName")
        {
            ErrorCode = 400;
            ErrorMessage = errorMessage;
        }

    }
}
