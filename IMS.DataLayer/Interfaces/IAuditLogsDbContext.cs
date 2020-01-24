using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IAuditLogsDbContext
    {
        void AddAuditLogs(string username, string action, string details, string performedOn, string remarks,string className);
        Task<Tuple<int, List<ActivityLogs>>> GetActivityLogs(int pageNumber, int pageSize, DateTime fromDate, DateTime toDate);

    }
}