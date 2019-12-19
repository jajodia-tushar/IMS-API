using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Validators
{
    public class VendorOrderValidator
    {
        public static bool Validate(VendorOrder vendorOrder)
        {
            if (vendorOrder == null)
                return false;
            Vendor vendor = vendorOrder.Vendor;
            if (vendor == null || vendor.Id <= 0)
                return false;
            VendorOrderDetails vendorOrderDetails = vendorOrder.VendorOrderDetails;
            if (vendorOrderDetails == null || string.IsNullOrEmpty(vendorOrderDetails.RecievedBy) || string.IsNullOrEmpty(vendorOrderDetails.SubmittedTo ) ||
                 vendorOrderDetails.TaxableAmount < 0)
                return false;
            List<ItemQuantityPriceMapping> orderItemDetails = vendorOrderDetails.OrderItemDetails;
            if (orderItemDetails == null || orderItemDetails.Count == 0)
                return false;
            foreach (var itemQtyPrice in orderItemDetails)
            {
                if (itemQtyPrice.Item.Id <= 0 || itemQtyPrice.Item.Rate <= 0 || itemQtyPrice.Quantity <= 0)
                    return false;

            }
            return true;

        }
        public static bool ValidatePlacedOrder(VendorOrder vendorOrder)
        {
            return Validate(vendorOrder);
        }

        public static bool ValidateApproveRequest(VendorOrder vendorOrder)
        {
            if (Validate(vendorOrder) && vendorOrder.VendorOrderDetails.OrderId > 0)
                foreach (var itemQtyPrice in vendorOrder.VendorOrderDetails.OrderItemDetails)
                    if (itemQtyPrice.TotalPrice <= 0)
                        return false;
            return true;

        }
    }
}
