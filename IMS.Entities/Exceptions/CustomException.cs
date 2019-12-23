using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Exceptions
{
    public class CustomException:Exception
    {
        public int ErrorCode=400;
        public string ErrorMessage = "Invalid Request";
    }
}
