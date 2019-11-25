using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class FailureResponse:Response
    {
        
        public string ErrorMessage { get; set; }
        public FailureResponse()
        {
            Status = Status.Failure;
        }
        public FailureResponse(string errorMessage)
        {
            Status = Status.Failure;
            ErrorMessage = errorMessage;
        }

    }
}
