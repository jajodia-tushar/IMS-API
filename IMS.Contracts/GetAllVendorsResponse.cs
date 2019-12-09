using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class GetAllVendorsResponse : Response
    {
        public List<Vendor> Vendors { get; set; }
    }
}
