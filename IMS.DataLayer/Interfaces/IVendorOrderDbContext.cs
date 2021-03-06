using IMS.DataLayer.Dto;
using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IVendorOrderDbContext
    {
        Task<bool> Delete(int orderId);
        Task<bool> Save(VendorOrder vendorOrder);
        Task<VendorOrdersDto> GetVendorOrders(bool isApproved, int pageNumber, int pageSize, DateTime startDate, DateTime endDate);
        Task<bool> ApproveOrder(VendorOrder vendorOrder);
        Task<VendorsOrderResponse> GetVendorOrdersByVendorId(int vendorId, int pageNumber, int pageSize, DateTime startDate, DateTime endDate);
        Task<VendorOrder> GetVendorOrdersByOrderId(int orderId);
        Task<bool> EditOrder(VendorOrder vendorOrder, User user);
        Task<bool> CheckUserEditedOrderBefore(int userId, int orderId);
        Task<string> GetLastOrderModifiedUser(int orderId);
    }
}
