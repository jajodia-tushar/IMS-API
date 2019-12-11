using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface ILogDbContext
    {
        void Log(int userId, string status, string callType, string severity, string request, string response);
        void LogException(string exceptionMessage, string excpetionType, string exceptionSource, string request, string response);
    }
}
