using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IMS.Entities;

namespace IMS.DataLayer.Interfaces
{
    public interface IEmployeeOrderDbContext
    {
        Task<EmployeeOrderResponse> GetRecentEmployeeOrders(int pageSize,int pageNumber);
        Task<EmployeeOrder> AddEmployeeOrder(EmployeeOrder employeeOrder);
        Task<EmployeeOrderResponse> GetEmployeeOrders(string employeeId, int limit, int offset, string startDate, string endDate);
       
    }
}
