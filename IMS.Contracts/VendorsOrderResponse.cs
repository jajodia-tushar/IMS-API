using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class VendorsOrderResponse : Response
    {
        public List<VendorOrder> VendorOrders { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
