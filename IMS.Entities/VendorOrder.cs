using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class VendorOrder
    {
        public Vendor Vendor { get; set; }
        public VendorOrderDetails VendorOrderDetails { get; set; }
    }
}
