using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Entities
{
   public class VendorOrderDetails
    {
        public int OrderId { get; set; }
        public bool IsApproved { get; set; }
        public string RecievedBy { get; set; }
        public string SubmittedTo { get; set; }
        public double FinalAmount { get; set; }
        public double TaxableAmount { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceImageUrl { get; set; }
        public DateTime Date { get; set; }
        public List<ItemQuantityPriceMapping> OrderItemDetails { get; set; }
    }
}
