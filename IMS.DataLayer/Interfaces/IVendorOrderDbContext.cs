using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.DataLayer.Interfaces
{
    public interface IVendorOrderDbContext
    {
        Task<bool> Delete(int orderId);
    }
}
