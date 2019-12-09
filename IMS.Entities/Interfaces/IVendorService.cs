using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities.Interfaces
{
    public interface IVendorService
    {
        VendorResponse GetVendorById(int vendorId);
        VendorResponse GetAllVendors();
    }
}
