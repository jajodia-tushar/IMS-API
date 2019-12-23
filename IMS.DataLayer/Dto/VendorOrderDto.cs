using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.DataLayer.Dto
{
    public class VendorOrderDto
    {
        public int VendorId { get; set; }
        public string VendorName { get; set; }
        public string VendorContactNumber { get; set; }
        public string VendorTitle { get; set; }
        public string VendorAddress { get; set; }
        public string VendorPAN { get; set; }
        public string VendorGST { get; set; }
        public string VendorCompanyIdentificationNumber { get; set; }
        public int OrderId { get; set; }
        public bool IsApproved { get; set; }
        public string RecievedBy { get; set; }
        public DateTime RecievedDate { get; set; }
        public string SubmittedTo { get; set; }
        public double TaxableAmount { get; set; }
        public string InvoiceNumber { get; set; }
        public string InvoiceImageUrl { get; set; }
        public string ChallanNumber { get; set; }
        public string ChallanImageUrl { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public int ItemMaxLimit { get; set; }
        public bool ItemIsActive { get; set; }
        public string ItemImageUrl { get; set; }

        public int Quantity { get; set; }
        public double TotalPrice { get; set; }
    }
}
