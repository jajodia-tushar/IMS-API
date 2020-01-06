using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class InvalidPagingInfo : CustomException
    {
        public InvalidPagingInfo(string errorMessage = "Invalid Paging Info")
        {
            ErrorCode = 400;
            ErrorMessage = errorMessage;
        }
    }
}
