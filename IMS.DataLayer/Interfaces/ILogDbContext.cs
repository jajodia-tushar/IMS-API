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
        void LogException(string callType, string request, string response, string stackTrace, string exceptionMessage, string innerException, string targetSite, string exceptionType, string severity);
        Task<List<Logs>> GetLogsRecord();
    }
}
