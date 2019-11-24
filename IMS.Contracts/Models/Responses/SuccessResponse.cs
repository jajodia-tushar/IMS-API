using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts.Models.Responses
{
    public class SuccessResponse:Response
    {
        public Object Data { set; get; }
        public SuccessResponse()
        {
            Status = Status.success;
        }
        public SuccessResponse(Object data)
        {
            Status = Status.success;
            Data = data;
        }
    }
}
