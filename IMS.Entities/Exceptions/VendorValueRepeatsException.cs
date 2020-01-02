using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class VendorValueRepeatsException : CustomException
    {
        public VendorValueRepeatsException(string errorMessage = "Values entered already exists")
        {
            ErrorCode = 422;
            ErrorMessage = errorMessage;
        }
    }
}
