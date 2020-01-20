using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Translators
{
    public class VendorOrderTranslator
    {
        /*
       * ToEntitiesObject 
       */
        public static Entities.VendorOrder ToEntitiesObject(Contracts.VendorOrder dtoVendorOrder)
        {
            if (dtoVendorOrder != null)
                return new Entities.VendorOrder
                {
                    Vendor = dtoVendorOrder.Vendor == null ? null : VendorTranslator.ToEntitiesObject(dtoVendorOrder.Vendor),
                    VendorOrderDetails = dtoVendorOrder.VendorOrderDetails == null ? null : ToEntitiesObject(dtoVendorOrder.VendorOrderDetails)
                };
            return null;
        }

        public static Entities.VendorOrderDetails ToEntitiesObject(Contracts.VendorOrderDetails dtoVendorOrderDetails)
        {
            if (dtoVendorOrderDetails != null)
                return new Entities.VendorOrderDetails
                {
                    OrderId = dtoVendorOrderDetails.OrderId,
                    IsApproved = dtoVendorOrderDetails.IsApproved,
                    RecievedBy = dtoVendorOrderDetails.RecievedBy,
                    SubmittedTo = dtoVendorOrderDetails.SubmittedTo,
                    TaxableAmount = dtoVendorOrderDetails.TaxableAmount,
                    InvoiceNumber = dtoVendorOrderDetails.InvoiceNumber,
                    InvoiceImageUrl = dtoVendorOrderDetails.InvoiceImageUrl == null ? "" : dtoVendorOrderDetails.InvoiceImageUrl,
                    ChallanImageUrl = dtoVendorOrderDetails.ChallanImageUrl == null ? "" : dtoVendorOrderDetails.ChallanImageUrl,
                    ChallanNumber = dtoVendorOrderDetails.ChallanNumber,
                    Date = dtoVendorOrderDetails.Date,
                    OrderItemDetails = dtoVendorOrderDetails.OrderItemDetails == null ? null : ToEntitiesObject(dtoVendorOrderDetails.OrderItemDetails)
                };
            return null;
        }
        public static List<Entities.ItemQuantityPriceMapping> ToEntitiesObject(List<Contracts.ItemQuantityPriceMapping> dtoOrderItemDetails)
        {
            List<Entities.ItemQuantityPriceMapping> entitiesOrderItemDetails = null;
            if (dtoOrderItemDetails != null)
            {
                entitiesOrderItemDetails = new List<Entities.ItemQuantityPriceMapping>();
                foreach (var itemQtyPrice in dtoOrderItemDetails)
                    entitiesOrderItemDetails.Add(ToEntitiesObject(itemQtyPrice));

            }
            return entitiesOrderItemDetails;
        }

        public static Entities.ItemQuantityPriceMapping ToEntitiesObject(Contracts.ItemQuantityPriceMapping itemQtyPrice)
        {
            if (itemQtyPrice != null)
            {
                return new Entities.ItemQuantityPriceMapping
                {
                    Item = itemQtyPrice == null ? null : ToEntitiesObject(itemQtyPrice.Item),
                    Quantity = itemQtyPrice.Quantity,
                    TotalPrice = itemQtyPrice.TotalPrice
                };
            }
            return null;
        }

        public static Entities.Item ToEntitiesObject(Contracts.Item dtoItem)
        {
            if (dtoItem != null)
                return new Entities.Item
                {
                    Id = dtoItem.Id,
                    Name = dtoItem.Name,
                    MaxLimit = dtoItem.MaxLimit,
                    IsActive = dtoItem.IsActive,
                    ImageUrl = dtoItem.ImageUrl,
                    Rate = dtoItem.Rate
                };
            return null;
        }



        /*
         * ToDataContractsObject 
         */
        public static Contracts.VendorsOrderResponse ToDataContractsObject(Entities.VendorsOrderResponse doGetListOfVendorOrdersResponse)
        {
            if (doGetListOfVendorOrdersResponse != null)
                return new Contracts.VendorsOrderResponse
                {
                    Status = doGetListOfVendorOrdersResponse.Status == Entities.Status.Success ? Contracts.Status.Success : Contracts.Status.Failure,
                    Error = doGetListOfVendorOrdersResponse.Error == null ? null : Translator.ToDataContractsObject(doGetListOfVendorOrdersResponse.Error),
                    VendorOrders = doGetListOfVendorOrdersResponse.VendorOrders == null ? null : ToDataContractsObject(doGetListOfVendorOrdersResponse.VendorOrders),
                    PagingInfo = doGetListOfVendorOrdersResponse.PagingInfo == null ? null : Translator.ToDataContractsObject(doGetListOfVendorOrdersResponse.PagingInfo)
                };

            return null;
        }
        public static Contracts.VendorOrderResponse ToDataContractsObject(Entities.VendorOrderResponse doVendorOrderResponse)
        {
            if (doVendorOrderResponse != null)
                return new Contracts.VendorOrderResponse
                {
                    Status = doVendorOrderResponse.Status == Entities.Status.Success ? Contracts.Status.Success : Contracts.Status.Failure,
                    Error = doVendorOrderResponse.Error == null ? null : Translator.ToDataContractsObject(doVendorOrderResponse.Error),
                    VendorOrder = doVendorOrderResponse.VendorOrder == null ? null : ToDataContractsObject(doVendorOrderResponse.VendorOrder),
                    CanEdit = doVendorOrderResponse.CanEdit == false ? false : true
                };
            return null;
        }



        private static List<Contracts.VendorOrder> ToDataContractsObject(List<Entities.VendorOrder> doListOfVendorOrders)
        {
            List<Contracts.VendorOrder> contractsListOfVendorOrders = null;
            if (doListOfVendorOrders != null)
            {
                contractsListOfVendorOrders = new List<Contracts.VendorOrder>();
                foreach (Entities.VendorOrder vendorOrder in doListOfVendorOrders)
                    contractsListOfVendorOrders.Add(ToDataContractsObject(vendorOrder));
                return contractsListOfVendorOrders;
            }

            return contractsListOfVendorOrders;
        }

        public static Contracts.VendorOrder ToDataContractsObject(Entities.VendorOrder vendorOrder)
        {
            if (vendorOrder != null)
                return new Contracts.VendorOrder
                {
                    Vendor = vendorOrder.Vendor == null ? null : VendorTranslator.ToDataContractsObject(vendorOrder.Vendor),
                    VendorOrderDetails = vendorOrder.VendorOrderDetails == null ? null : ToDataContractsObject(vendorOrder.VendorOrderDetails)
                };
            return null;
        }

        public static Contracts.VendorOrderDetails ToDataContractsObject(Entities.VendorOrderDetails doVendorOrderDetails)
        {
            if (doVendorOrderDetails != null)
                return new Contracts.VendorOrderDetails
                {
                    OrderId = doVendorOrderDetails.OrderId,
                    IsApproved = doVendorOrderDetails.IsApproved,
                    RecievedBy = doVendorOrderDetails.RecievedBy,
                    SubmittedTo = doVendorOrderDetails.SubmittedTo,
                    TaxableAmount = doVendorOrderDetails.TaxableAmount,
                    InvoiceNumber = doVendorOrderDetails.InvoiceNumber,
                    InvoiceImageUrl = doVendorOrderDetails.InvoiceImageUrl,
                    ChallanNumber = doVendorOrderDetails.ChallanNumber,
                    ChallanImageUrl = doVendorOrderDetails.ChallanImageUrl,
                    Date = doVendorOrderDetails.Date,
                    OrderItemDetails = doVendorOrderDetails.OrderItemDetails == null ? null : ToDataContractsObject(doVendorOrderDetails.OrderItemDetails)
                };
            return null;
        }
        public static List<Contracts.ItemQuantityPriceMapping> ToDataContractsObject(List<Entities.ItemQuantityPriceMapping> doOrderItemDetails)
        {
            List<Contracts.ItemQuantityPriceMapping> contractsOrderItemDetails = null;
            if (doOrderItemDetails != null)
            {
                contractsOrderItemDetails = new List<Contracts.ItemQuantityPriceMapping>();
                foreach (var itemQtyPrice in doOrderItemDetails)
                    contractsOrderItemDetails.Add(ToDataContractsObject(itemQtyPrice));

            }
            return contractsOrderItemDetails;
        }

        public static Contracts.ItemQuantityPriceMapping ToDataContractsObject(Entities.ItemQuantityPriceMapping doItemQtyPrice)
        {
            if (doItemQtyPrice != null)
            {
                return new Contracts.ItemQuantityPriceMapping
                {
                    Item = doItemQtyPrice == null ? null : ToDataContractsObject(doItemQtyPrice.Item),
                    Quantity = doItemQtyPrice.Quantity,
                    TotalPrice = doItemQtyPrice.TotalPrice
                };
            }
            return null;
        }

        public static Contracts.Item ToDataContractsObject(Entities.Item doItem)
        {
            if (doItem != null)
                return new Contracts.Item
                {
                    Id = doItem.Id,
                    Name = doItem.Name,
                    MaxLimit = doItem.MaxLimit,
                    IsActive = doItem.IsActive,
                    ImageUrl = doItem.ImageUrl,
                    Rate = doItem.Rate
                };
            return null;
        }
    }
}
