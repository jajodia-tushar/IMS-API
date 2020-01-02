using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class VendorsResponse :Response
    {
        public List<Vendor> Vendors { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
