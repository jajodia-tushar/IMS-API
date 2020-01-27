using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IMailService
    {
        Task<bool> SendEmployeeOrderReciept(EmployeeOrder employeeOrder);
        Task<bool> SendEmployeeBulkOrderReciept(EmployeeBulkOrder employeeBulkOrder, BulkOrderRequestStatus orderStatus);
        Task<bool> SendApprovedBulkOrderToLoggedInUser(int loggedInUserId, EmployeeBulkOrder order, ApproveEmployeeBulkOrder approveEmployeeBulkOrder);
    }
}
