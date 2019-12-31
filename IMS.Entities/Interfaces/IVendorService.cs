using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Entities.Interfaces
{
    public interface IVendorService
    {
       Task<VendorResponse> GetVendorById(int vendorId);
       Task<VendorResponse>  GetAllVendors();
       Task<VendorsResponse> SearchByName(string name, int pageNumber, int pageSize);
    }
}
