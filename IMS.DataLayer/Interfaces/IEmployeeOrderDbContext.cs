using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IMS.Entities;

namespace IMS.DataLayer.Interfaces
{
    public interface IEmployeeOrderDbContext
    {
        Task<EmployeeRecentOrderResponse> GetRecentEmployeeOrders(int pageSize,int pageNumber);
        Task<EmployeeOrder> AddEmployeeOrder(EmployeeOrder employeeOrder);
        Task<List<EmployeeOrderDetails>> GetOrdersByEmployeeId(string employeeId);
       
    }
}
