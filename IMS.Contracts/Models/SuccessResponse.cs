using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts.Models
{
    public class SuccessResponse:Response
    {
        public Object Data { set; get; }
        public SuccessResponse()
        {
            Status = Status.Success;
        }
        public SuccessResponse(Object data)
        {
            Status = Status.Success;
            Data = data;
        }
    }
}
