using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class GetVendorResponse :Response
    {
        public Vendor Vendor { get; set; }
    }
}
