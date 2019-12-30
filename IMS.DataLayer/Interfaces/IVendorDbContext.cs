using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IVendorDbContext
    {
        Task<Vendor> GetVendorById(int vendorId);
        Task<VendorsResponse> GetVendors(string name, int limit, int offset);
        Task<Vendor> AddVendor(Vendor vendor);
        Task<Vendor> UpdateVendor(Vendor vendor);
        Task<bool> DeleteVendor(int vendorId, bool isHardDelete);
    }
}
