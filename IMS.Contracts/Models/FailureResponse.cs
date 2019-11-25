using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts.Models
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
