using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IMS.Entities;

namespace IMS.DataLayer.Interfaces
{
    public interface IEmployeeBulkOrderDbContext
    {
        Task<Tuple<int,List<EmployeeBulkOrder>>> GetAllEmployeeBulkOrders(int pageNumber, int pageSize,DateTime fromDate, DateTime endDate);
        Task<bool> SaveOrder(EmployeeBulkOrder employeeBulkOrder);
        Task<EmployeeBulkOrder> GetOrderById(int orderId);
        Task<EmployeeBulkOrder> CancelOrReturnOrderItems (EmployeeBulkOrder employeeBulkOrder);
        Task<bool> ApproveOrder(ApproveEmployeeBulkOrder requestedApproveEmployeeBulkOrder);
        Task<bool> RejectOrder(int orderId);
    }
}
