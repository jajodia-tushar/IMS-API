using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IVendorService
    {
       Task<VendorResponse> GetVendorById(int vendorId);
       Task<VendorsResponse> GetVendors(string name, int pageNumber, int pageSize);
       Task<VendorResponse> UpdateVendor(Vendor vendor);
       Task<VendorResponse> AddVendor(Vendor vendor);
        Task<Response> DeleteVendor(int vendorId, bool isHardDelete);
    }
}
