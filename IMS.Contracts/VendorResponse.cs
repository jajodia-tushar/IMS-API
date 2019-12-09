using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class VendorResponse :Response
    {
        public List<Vendor> Vendors { get; set; }
    }
}
