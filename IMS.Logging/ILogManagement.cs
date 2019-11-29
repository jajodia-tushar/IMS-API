using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Logging
{
    public interface ILogManagement
    {
        void Log( Object request, Response response);
    }
}
