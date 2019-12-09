using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
    public class GetAllVendorsResponse : Response
    {
        public List<Vendor> Vendors { get; set; }
    }
}
