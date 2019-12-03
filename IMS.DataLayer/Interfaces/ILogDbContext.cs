using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface ILogDbContext
    {
        Task Log(int userId, string status, string callType, string severity, string request, string response);

    }
}
