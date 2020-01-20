using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class VendorOrderResponse : Response
    {
        public VendorOrder VendorOrder { get; set; }
        public bool CanEdit { get; set; }
    }
}
