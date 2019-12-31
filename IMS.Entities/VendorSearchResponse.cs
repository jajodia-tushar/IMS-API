using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class VendorSearchResponse : Response
    {
        public PagingInfo PagingInfo { get; set; }
        public List<Vendor> Vendors { get; set; }
    }
}
