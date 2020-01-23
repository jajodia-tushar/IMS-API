using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public class EmployeeBulkOrderTranslator
    {
        public static Entities.EmployeeBulkOrder ToEntitiesObject(Contracts.EmployeeBulkOrder employeeBulkOrder)
        {
            if (employeeBulkOrder == null)
                return null;
            return new Entities.EmployeeBulkOrder()
            {
                BulkOrderId = employeeBulkOrder.BulkOrderId,
                Employee = EmployeeTranslator.ToEntitiesObject(employeeBulkOrder.Employee),
                EmployeeBulkOrderDetails = ToEntitiesObject(employeeBulkOrder.EmployeeBulkOrderDetails)
            };
        }

        public static Entities.EmployeeBulkOrderDetails ToEntitiesObject(Contracts.EmployeeBulkOrderDetails employeeBulkOrderDetails)
        {
            if (employeeBulkOrderDetails == null)
                return null;
            return new Entities.EmployeeBulkOrderDetails()
            {
                CreatedOn = employeeBulkOrderDetails.CreatedOn,
                RequirementDate = employeeBulkOrderDetails.RequirementDate,
                BulkOrderRequestStatus = (Entities.BulkOrderRequestStatus)(employeeBulkOrderDetails.BulkOrderRequestStatus),
                ReasonForRequirement = employeeBulkOrderDetails.ReasonForRequirement,
                ItemsQuantityList = ToEntitiesObject(employeeBulkOrderDetails.ItemsQuantityList)
            };
        }

        public static List<Entities.BulkOrderItemQuantityMapping> ToEntitiesObject(List<Contracts.BulkOrderItemQuantityMapping> itemsQuantityList)
        {
            if (itemsQuantityList == null)
                return null;
            List<Entities.BulkOrderItemQuantityMapping> entitiesItemsQuantityList = new List<Entities.BulkOrderItemQuantityMapping>();
            foreach(Contracts.BulkOrderItemQuantityMapping itemQuantity in itemsQuantityList)
            {
                entitiesItemsQuantityList.Add(ToEntitiesObject(itemQuantity));
            }
            return entitiesItemsQuantityList;
        }

        private static Entities.BulkOrderItemQuantityMapping ToEntitiesObject(Contracts.BulkOrderItemQuantityMapping itemQuantity)
        {
            if (itemQuantity == null)
                return null;
            return new Entities.BulkOrderItemQuantityMapping
            {
                Item = itemQuantity.Item==null?null:ItemTranslator.ToEntitiesObject(itemQuantity.Item),
                QuantityOrdered=itemQuantity.QuantityOrdered,
                QuantityUsed=itemQuantity.QuantityUsed
            };

        }

        public static Contracts.EmployeeBulkOrdersResponse ToDataContractsObject(Entities.EmployeeBulkOrdersResponse entityBulkOrdersResponse)
        {
            Contracts.EmployeeBulkOrdersResponse contractBulkOrderResponse = new Contracts.EmployeeBulkOrdersResponse();
            if (entityBulkOrdersResponse.Status == Entities.Status.Success)
            {
                contractBulkOrderResponse.Status = Contracts.Status.Success;
                contractBulkOrderResponse.EmployeeBulkOrders = ToDataContractsObject(entityBulkOrdersResponse.EmployeeBulkOrders);
                
                contractBulkOrderResponse.PagingInfo = Translator.ToDataContractsObject(entityBulkOrdersResponse.PagingInfo);

            }
            else
            {
                contractBulkOrderResponse.Status = Contracts.Status.Failure;
                contractBulkOrderResponse.Error = Translator.ToDataContractsObject(entityBulkOrdersResponse.Error);
            }
            return contractBulkOrderResponse;

        }

        public static List<Contracts.EmployeeBulkOrder> ToDataContractsObject(List<Entities.EmployeeBulkOrder> employeeBulkOrders)
        {
            if (employeeBulkOrders == null)
                return null;
            List<Contracts.EmployeeBulkOrder> entityEmployeeBulkOrders = new List<Contracts.EmployeeBulkOrder>();
            foreach (Entities.EmployeeBulkOrder order in employeeBulkOrders)
            {
                entityEmployeeBulkOrders.Add(ToDataContractsObject(order));
            }
            return entityEmployeeBulkOrders;
        }

        public static Contracts.EmployeeBulkOrder ToDataContractsObject(Entities.EmployeeBulkOrder employeeBulkOrder)
        {
            if (employeeBulkOrder == null)
                return null;
            return new Contracts.EmployeeBulkOrder()
            {
                BulkOrderId = employeeBulkOrder.BulkOrderId,
                Employee = EmployeeTranslator.ToDataContractsObject(employeeBulkOrder.Employee),
                EmployeeBulkOrderDetails = ToDataContractsObject(employeeBulkOrder.EmployeeBulkOrderDetails)
            };
        }

        public static Contracts.EmployeeBulkOrderDetails ToDataContractsObject(Entities.EmployeeBulkOrderDetails employeeBulkOrderDetails)
        {
            if (employeeBulkOrderDetails == null)
                return null;
            return new Contracts.EmployeeBulkOrderDetails()
            {
                CreatedOn = employeeBulkOrderDetails.CreatedOn,
                RequirementDate = employeeBulkOrderDetails.RequirementDate,
                BulkOrderRequestStatus = (Contracts.BulkOrderRequestStatus)(employeeBulkOrderDetails.BulkOrderRequestStatus),
                ReasonForRequirement = employeeBulkOrderDetails.ReasonForRequirement,
                ItemsQuantityList = ToDataContractsObject(employeeBulkOrderDetails.ItemsQuantityList)
            };
        }

        public static List<Contracts.BulkOrderItemQuantityMapping> ToDataContractsObject(List<Entities.BulkOrderItemQuantityMapping> itemsQuantityList)
        {
            if (itemsQuantityList == null)
                return null;
            List<Contracts.BulkOrderItemQuantityMapping> contractsItemsQuantityList = new List<Contracts.BulkOrderItemQuantityMapping>();
            foreach (Entities.BulkOrderItemQuantityMapping itemQuantity in itemsQuantityList)
            {
                contractsItemsQuantityList.Add(ToDataContractsObject(itemQuantity));
            }
            return contractsItemsQuantityList;
        }

        public static Contracts.BulkOrderItemQuantityMapping ToDataContractsObject(Entities.BulkOrderItemQuantityMapping itemQuantity)
        {
            if (itemQuantity == null)
                return null;
            return new Contracts.BulkOrderItemQuantityMapping
            {
                Item = itemQuantity.Item == null ? null : ItemTranslator.ToDataContractsObject(itemQuantity.Item),
                QuantityOrdered = itemQuantity.QuantityOrdered,
                QuantityUsed = itemQuantity.QuantityUsed
            };
        }
    }
}
