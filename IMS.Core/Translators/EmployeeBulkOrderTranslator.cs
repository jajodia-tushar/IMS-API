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
                EmployeeItemsQuantityList = ShelfItemsTranslator.ToEntitiesObject(employeeBulkOrderDetails.EmployeeItemsQuantityList)
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
                EmployeeItemsQuantityList = ShelfItemsTranslator.ToDataContractsObject(employeeBulkOrderDetails.EmployeeItemsQuantityList)
            };
        }
    }
}
