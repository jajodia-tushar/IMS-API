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
       
        public double TaxableAmount { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceImageUrl { get; set; }
        public DateTime Date { get; set; }
        public List<ItemQuantityPriceMapping> OrderItemDetails { get; set; }
        public double FinalAmount
        {
            get
            {
                double totalAmount = 0;
                if (this.OrderItemDetails != null && this.OrderItemDetails.Count > 0)
                {
                    foreach (ItemQuantityPriceMapping itemQtyPrice in this.OrderItemDetails)
                        totalAmount += itemQtyPrice.TotalPrice;
                    totalAmount += this.TaxableAmount;
                }
                return Math.Round(totalAmount,2);
            }
        }
    }
}
