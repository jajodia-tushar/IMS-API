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
        Task<List<VendorOrder>> GetAllPendingApprovals(int pageNumber, int pageSize);
        Task<bool> ApproveOrder(VendorOrder vendorOrder);
    }
}
