using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class VendorsResponse : Response
    {
        public PagingInfo PagingInfo { get; set; }
        public List<Vendor> Vendors { get; set; }
    }
}
