using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class VendorsOrderResponse : Response
    {
        public List<VendorOrder> VendorOrders { get; set; }
    }
}
