using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class ListOfVendorOrdersResponse : Response
    {
        public List<VendorOrder> ListOfVendorOrders { get; set; }
    }
}
