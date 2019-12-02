using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Interfaces
{
    public interface ILogDbContext
    {
       void Log(int userId, string status, string callType, string severity, string request, string response);

    }
}
