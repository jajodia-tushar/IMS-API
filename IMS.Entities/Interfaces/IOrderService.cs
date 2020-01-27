using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IOrderService
    {

        Task<Response> Delete(int orderId);
        Task<EmployeeOrderResponse> GetEmployeeRecentOrders(int pageNumber, int pageSize);
        Task<EmployeeOrderResponse> GetEmployeeOrders(string employeeId, int pageNumber, int pageSize, string startDate, string endDate);
        Task<EmployeeOrderResponse> PlaceEmployeeOrder(EmployeeOrder employeeOrder);
        Task<VendorOrderResponse> SaveVendorOrder(VendorOrder vendorOrder);
        Task<VendorsOrderResponse> GetVendorOrders(bool isApproved, int pageNumber, int pageSize, string fromDate, string toDate);
        Task<VendorOrderResponse> ApproveVendorOrder(VendorOrder vendorOrder);
        Task<VendorsOrderResponse> GetVendorOrdersByVendorId(int vendorId, int pageNumber, int pageSize, string fromDate, string toDate);
        Task<VendorOrderResponse> GetVendorOrderByOrderId(int orderId);
        Task<EmployeeBulkOrdersResponse> GetEmployeeBulkOrders(int? pageNumber, int? pageSize, string fromDate, string toDate);
        Task<EmployeeBulkOrdersResponse> PlaceEmployeeBulkOrder(EmployeeBulkOrder employeeBulkOrder);
        Task<EmployeeBulkOrdersResponse> GetEmployeeBulkOrderById(int orderid);
        Task<ApproveBulkOrderResponse> ApproveEmployeeBulkOrder(int orderid, ApproveEmployeeBulkOrder approveEmployeeBulkOrder);
        Task<Response> RejectEmployeeBulkOrder(int orderId);
        Task<EmployeeBulkOrdersResponse> ReturnOrderItems(int orderId, EmployeeBulkOrder employeeBulkOrder);
        Task<EmployeeBulkOrdersResponse> CancelEmployeeBulkOrder(int orderid);
    }
}