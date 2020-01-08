using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class InvalidVendorException : CustomException
    {
        public InvalidVendorException(string errorMessage = "Values entered already exists")
        {
            ErrorCode = 422;
            ErrorMessage = errorMessage;
        }
    }
}
