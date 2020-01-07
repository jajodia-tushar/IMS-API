using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Dto
{
    public class VendorOrdersDto
    {
        public List<VendorOrder> VendorOrders{get;set;}
        public int TotalNumberOfVendorOrders { get; set; }
    }
}
