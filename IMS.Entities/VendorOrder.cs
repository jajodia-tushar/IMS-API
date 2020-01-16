using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    [Serializable]
    public class VendorOrder
    {
        public Vendor Vendor { get; set; }
        public VendorOrderDetails VendorOrderDetails { get; set; }
    }
}
