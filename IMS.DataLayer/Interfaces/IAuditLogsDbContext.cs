using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Interfaces
{
    public interface IAuditLogsDbContext
    {
        void AddAuditLogs(string username, string action, string details, string performedOn, string remarks,string className);
    }
}