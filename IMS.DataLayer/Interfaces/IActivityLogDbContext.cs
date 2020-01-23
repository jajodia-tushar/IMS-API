using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IActivityLogDbContext
    {
        Task<Tuple<int, List<ActivityLogs>>> GetActivityLogs(int pageNumber, int pageSize, DateTime fromDate, DateTime toDate);
    }
}
