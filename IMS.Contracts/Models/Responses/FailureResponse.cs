using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts.Models.Responses
{
    public class FailureResponse:Response
    {
        
        public string ErrorMessage { get; set; }
        public FailureResponse()
        {
            Status = Status.failure;
        }
        public FailureResponse(string errorMessage)
        {
            Status = Status.failure;
            ErrorMessage = errorMessage;
        }

    }
}
