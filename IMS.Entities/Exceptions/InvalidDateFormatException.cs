using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class InvalidDateFormatException : CustomException
    {
        public InvalidDateFormatException(string errorMessage = "Invalid Date Format")
        {
            ErrorCode = 400;
            ErrorMessage = errorMessage;
        }
    }
}
