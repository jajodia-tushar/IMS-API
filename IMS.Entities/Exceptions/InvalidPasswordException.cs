using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class InvalidPasswordException:CustomException
    {
        public InvalidPasswordException(string errorMessaage = "InvalidPasswordFormat")
        {
            ErrorCode = 400;
            ErrorMessage = errorMessaage;
        }
    }
}
