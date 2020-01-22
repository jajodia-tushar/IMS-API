using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class RecordsNotFoundException : CustomException
    {
        public RecordsNotFoundException(string errorMessage = "Records Not Found")
        {
            ErrorCode = 404;
            ErrorMessage = errorMessage;
        }
    }
}
