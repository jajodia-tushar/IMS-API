using IMS.Entities;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Logging
{
    public interface ILogManager
    {
        void Log(Object request, Response response, int UserId);
    }
}
