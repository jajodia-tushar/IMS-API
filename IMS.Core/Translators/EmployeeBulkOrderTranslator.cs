using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public class EmployeeBulkOrderTranslator
    {
        public static Entities.ApproveEmployeeBulkOrder ToEntitiesObject(Contracts.ApproveEmployeeBulkOrder contractsApproveEmployeeBulkOrder)
        {
            if (contractsApproveEmployeeBulkOrder == null)
                return null;
            return new Entities.ApproveEmployeeBulkOrder()
            {
                BulkOrderId = contractsApproveEmployeeBulkOrder.BulkOrderId,
                Employee = EmployeeTranslator.ToEntitiesObject(contractsApproveEmployeeBulkOrder.Employee),
                EmployeeBulkOrderDetails = ToEntitiesObject(contractsApproveEmployeeBulkOrder.EmployeeBulkOrderDetails),
                ItemLocationQuantityMappings=ToEntitiesObject(contractsApproveEmployeeBulkOrder.ItemLocationQuantityMappings)
            };
        }

        public static List<Entities.ItemLocationQuantityMapping> ToEntitiesObject(List<Contracts.ItemLocationQuantityMapping> contractsItemLocationQuantityMappings)
        {
            if (contractsItemLocationQuantityMappings == null)
                return null;
            List<Entities.ItemLocationQuantityMapping> entitiesItemLocationQuantityMappings = new List<Entities.ItemLocationQuantityMapping>();
            foreach(Contracts.ItemLocationQuantityMapping itemLocationQuantityMapping in contractsItemLocationQuantityMappings)
            {
                entitiesItemLocationQuantityMappings.Add(ToEntitiesObject(itemLocationQuantityMapping));
            }
            return entitiesItemLocationQuantityMappings;
        }

        public static Entities.ItemLocationQuantityMapping ToEntitiesObject(Contracts.ItemLocationQuantityMapping contractsItemLocationQuantityMapping)
        {
            if (contractsItemLocationQuantityMapping == null)
                return null;
            return new Entities.ItemLocationQuantityMapping
            {
                Item = ItemTranslator.ToEntitiesObject(contractsItemLocationQuantityMapping.Item),
                LocationQuantityMappings = ToEntitiesObject(contractsItemLocationQuantityMapping.LocationQuantityMappings)
            };
        }

        public static List<Entities.LocationQuantityMapping> ToEntitiesObject(List<Contracts.LocationQuantityMapping> contractsLocationQuantityMappings)
        {
            if (contractsLocationQuantityMappings == null)
                return null;
            List<Entities.LocationQuantityMapping> entitiesLocationQuantityMappings = new List<Entities.LocationQuantityMapping>();
            foreach (Contracts.LocationQuantityMapping LocationQuantityMapping in contractsLocationQuantityMappings)
            {
                entitiesLocationQuantityMappings.Add(ToEntitiesObject(LocationQuantityMapping));
            }
            return entitiesLocationQuantityMappings;
        }

        public static Entities.LocationQuantityMapping ToEntitiesObject(Contracts.LocationQuantityMapping contractsLocationQuantityMapping)
        {
            if (contractsLocationQuantityMapping == null)
                return null;
            return new Entities.LocationQuantityMapping
            {
                Location=contractsLocationQuantityMapping.Location,
                Quantity=contractsLocationQuantityMapping.Quantity
            };
        }

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
      
         public static Contracts.ApproveBulkOrderResponse ToDataContractsObject(Entities.ApproveBulkOrderResponse entitiyApproveBulkOrderResponse)
        {
            Contracts.ApproveBulkOrderResponse contractApproveBulkOrderResponse = new Contracts.ApproveBulkOrderResponse();
            if (entitiyApproveBulkOrderResponse.Status == Entities.Status.Success)
            {
                contractApproveBulkOrderResponse.Status = Contracts.Status.Success;
                contractApproveBulkOrderResponse.ApproveEmployeeBulkOrder = ToDataContractsObject(entitiyApproveBulkOrderResponse.ApproveEmployeeBulkOrder);

            }
            else
            {
                contractApproveBulkOrderResponse.Status = Contracts.Status.Failure;
                contractApproveBulkOrderResponse.Error = Translator.ToDataContractsObject(entitiyApproveBulkOrderResponse.Error);
            }
            return contractApproveBulkOrderResponse;

        }
        public static Contracts.ApproveEmployeeBulkOrder ToDataContractsObject(Entities.ApproveEmployeeBulkOrder entitiesApproveEmployeeBulkOrder)
        {
            if (entitiesApproveEmployeeBulkOrder == null)
                return null;
            return new Contracts.ApproveEmployeeBulkOrder()
            {
                BulkOrderId = entitiesApproveEmployeeBulkOrder.BulkOrderId,
                Employee = EmployeeTranslator.ToDataContractsObject(entitiesApproveEmployeeBulkOrder.Employee),
                EmployeeBulkOrderDetails = ToDataContractsObject(entitiesApproveEmployeeBulkOrder.EmployeeBulkOrderDetails),
                ItemLocationQuantityMappings = ToDataContractsObject(entitiesApproveEmployeeBulkOrder.ItemLocationQuantityMappings)
            };
        }

        public static List<Contracts.ItemLocationQuantityMapping> ToDataContractsObject(List<Entities.ItemLocationQuantityMapping> entityItemLocationQuantityMappings)
        {
            if (entityItemLocationQuantityMappings == null)
                return null;
            List<Contracts.ItemLocationQuantityMapping> contractsItemLocationQuantityMappings = new List<Contracts.ItemLocationQuantityMapping>();
            foreach (Entities.ItemLocationQuantityMapping itemLocationQuantityMapping in entityItemLocationQuantityMappings)
            {
                contractsItemLocationQuantityMappings.Add(ToDataContractsObject(itemLocationQuantityMapping));
            }
            return contractsItemLocationQuantityMappings;
        }

        public static Contracts.ItemLocationQuantityMapping ToDataContractsObject(Entities.ItemLocationQuantityMapping entitiesItemLocationQuantityMapping)
        {
            if (entitiesItemLocationQuantityMapping == null)
                return null;
            return new Contracts.ItemLocationQuantityMapping
            {
                Item = ItemTranslator.ToDataContractsObject(entitiesItemLocationQuantityMapping.Item),
                LocationQuantityMappings = ToDataContractsObject(entitiesItemLocationQuantityMapping.LocationQuantityMappings)
            };
        }

        public static List<Contracts.LocationQuantityMapping> ToDataContractsObject(List<Entities.LocationQuantityMapping> entityLocationQuantityMappings)
        {
            if (entityLocationQuantityMappings == null)
                return null;
            List<Contracts.LocationQuantityMapping> contractsLocationQuantityMappings = new List<Contracts.LocationQuantityMapping>();
            foreach (Entities.LocationQuantityMapping LocationQuantityMapping in entityLocationQuantityMappings)
            {
                contractsLocationQuantityMappings.Add(ToDataContractsObject(LocationQuantityMapping));
            }
            return contractsLocationQuantityMappings;
        }

        public static Contracts.LocationQuantityMapping ToDataContractsObject(Entities.LocationQuantityMapping entitiesLocationQuantityMapping)
        {
            if (entitiesLocationQuantityMapping == null)
                return null;
            return new Contracts.LocationQuantityMapping
            {
                Location = entitiesLocationQuantityMapping.Location,
                Quantity = entitiesLocationQuantityMapping.Quantity
            };
        }
    }
}
