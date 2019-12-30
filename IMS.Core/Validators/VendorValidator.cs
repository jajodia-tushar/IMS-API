using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Validators
{
    public class VendorValidator
    {
        public static bool Validate(Vendor vendor)
        {
            if (vendor == null||
                vendor.Id <= 0||
                String.IsNullOrEmpty(vendor.Name) || String.IsNullOrEmpty(vendor.PAN) || String.IsNullOrEmpty(vendor.Title) || String.IsNullOrEmpty(vendor.GST) 
                || String.IsNullOrEmpty(vendor.Address) || String.IsNullOrEmpty(vendor.CompanyIdentificationNumber) || String.IsNullOrEmpty(vendor.ContactNumber))
                return false;
            return true;
        }
    }
}
