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
        Task<int> GetVendors(string name, int limit, int offset, List<Vendor> vendors);
    }
}
