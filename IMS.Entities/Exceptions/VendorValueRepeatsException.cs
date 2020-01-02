using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class InValidVendorException : CustomException
    {
        public InValidVendorException(string errorMessage = "Values entered already exists")
        {
            ErrorCode = 422;
            ErrorMessage = errorMessage;
        }
    }
}
