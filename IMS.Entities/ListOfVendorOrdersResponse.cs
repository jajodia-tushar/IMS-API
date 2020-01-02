using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class ListOfVendorOrdersResponse : Response
    {
        public List<VendorOrder> ListOfVendorOrders { get; set; }
    }
}
