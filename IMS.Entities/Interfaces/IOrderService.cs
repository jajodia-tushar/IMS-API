using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IOrderService
    {

        Task<Response> Delete(int orderId);
        Task<EmployeeRecentOrderResponse> GetEmployeeRecentOrders(int pageNumber, int pageSize);
        Task<OrdersByEmployeeIdResponse> GetEmployeeOrders(string employeeId);
        Task<EmployeeOrderResponse> PlaceEmployeeOrder(EmployeeOrder employeeOrder);
        Task<VendorOrderResponse> SaveVendorOrder(VendorOrder vendorOrder);
        Task<GetListOfVendorOrdersResponse> GetAllVendorOrderPendingApprovals(int pageNumber, int pageSize);
        Task<Response> ApproveVendorOrder(VendorOrder vendorOrder);
    }
}