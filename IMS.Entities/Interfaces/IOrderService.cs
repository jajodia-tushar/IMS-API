using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IOrderService
    {
<<<<<<< HEAD
        Task<Response> Delete(int orderId);
=======
        Task<EmployeeRecentOrderResponse> GetEmployeeRecentOrders(int pageNumber, int pageSize);
>>>>>>> Added Controller
    }
}
