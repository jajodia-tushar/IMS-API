using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Contracts
{
    public class Vendor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactNumber { get; set; }
        public string Title { get; set; }
        public string Address { get; set; }
        public string PAN { get; set; }

        public string GST { get; set; }

        public string CompanyIdentificationNumber { get; set; }
    }
}
