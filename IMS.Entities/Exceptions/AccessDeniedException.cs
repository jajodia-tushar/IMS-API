using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class AccessDeniedException:CustomException
    {
        public AccessDeniedException(string errorMessage = "Access Denied")
        {
            ErrorCode = 403;
            ErrorMessage = errorMessage;
        }
    }
}
