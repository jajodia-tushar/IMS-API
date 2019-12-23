using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class GetListOfVendorOrdersResponse : Response
    {
        public List<VendorOrder> ListOfVendorOrders { get; set; }
    }
}
