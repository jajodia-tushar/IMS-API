using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class EmployeeIdAlreadyExists:CustomException
    {
        public EmployeeIdAlreadyExists(string errorMessage="Invalid Employee ID")
        {
            ErrorCode = 400;
            ErrorMessage = errorMessage;
        }
    }
}
