using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IMS.Entities;

namespace IMS.DataLayer.Interfaces
{
    public interface IEmployeeOrderDbContext
    {
        Task<List<EmployeeRecentOrder>> GetRecentEmployeeOrders(int pageNumber, int pageSize);
        Task<EmployeeOrder> AddEmployeeOrder(EmployeeOrder employeeOrder);
        Task<List<EmployeeOrderDetails>> GetOrdersByEmployeeId(string employeeId);
        Task<PagingInfo> GetEmployeeOrderCount(int pageNumber, int pageSize);
    }
}
