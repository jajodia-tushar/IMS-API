using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public class EmployeeOrderTranslator
    {
        public static Entities.EmployeeOrderDetails ToEntitiesObject(Contracts.EmployeeOrderDetails employeeOrder)
        {
            if (employeeOrder != null)
            {
                return new Entities.EmployeeOrderDetails()
                {
                    Date = employeeOrder.Date,
                    OrderId = employeeOrder.OrderId,
                    Shelf = employeeOrder.Shelf==null?null:ShelfTranslator.ToEntitiesObject(employeeOrder.Shelf),
                    EmployeeItemsQuantityList =employeeOrder.EmployeeItemsQuantityList==null?null:
                    ShelfItemsTranslator.ToEntitiesObject(employeeOrder.EmployeeItemsQuantityList)
                };
            }
            return null;
        }
        public static Contracts.EmployeeOrderResponse ToDataContractsObject(Entities.EmployeeOrderResponse placeEmployeeOrderResponseEntity)
        {
            if (placeEmployeeOrderResponseEntity != null)
            {
                return new Contracts.EmployeeOrderResponse
                {
                    Error = placeEmployeeOrderResponseEntity.Error == null ? null : 
                    Translator.ToDataContractsObject(placeEmployeeOrderResponseEntity.Error),
                    Status = placeEmployeeOrderResponseEntity.Status == Entities.Status.Success ? Contracts.Status.Success : 
                    Contracts.Status.Failure,
                    EmployeeOrders = placeEmployeeOrderResponseEntity.EmployeeOrders==null?null : 
                    ToDataContractsObject(placeEmployeeOrderResponseEntity.EmployeeOrders),
                    PagingInfo = placeEmployeeOrderResponseEntity.PagingInfo == null ? null :
                    Translator.ToDataContractsObject(placeEmployeeOrderResponseEntity.PagingInfo)
                };
            }
            return null;
        }

        private static List<Contracts.EmployeeOrder> ToDataContractsObject(List<Entities.EmployeeOrder> employeeOrders)
        {
            List<Contracts.EmployeeOrder> employeeOrdersContract = new List<Contracts.EmployeeOrder>();
            if(employeeOrders!=null && employeeOrders.Count!=0)
            {
                foreach (Entities.EmployeeOrder employeeOrder in employeeOrders )
                {
                    employeeOrdersContract.Add(ToDataContractsObject(employeeOrder));
                }
            }
            return employeeOrdersContract;
        }

        public static Contracts.EmployeeOrder ToDataContractsObject(Entities.EmployeeOrder employeeOrderEntity)
        {

            Contracts.EmployeeOrder contractEmployeeOrder = new Contracts.EmployeeOrder();
            if (employeeOrderEntity.Employee != null)
            {
                contractEmployeeOrder.Employee = EmployeeTranslator.ToDataContractsObject(employeeOrderEntity.Employee);
            }
            if (employeeOrderEntity.EmployeeOrderDetails != null)
            {
                contractEmployeeOrder.EmployeeOrderDetails = ToDataContractsObject(employeeOrderEntity.EmployeeOrderDetails);
            }
            return contractEmployeeOrder;
        }
        public static List<Contracts.EmployeeOrderDetails> ToDataContractsObject(List<Entities.EmployeeOrderDetails> employeeOrders)
        {
            List<Contracts.EmployeeOrderDetails> orders = new List<Contracts.EmployeeOrderDetails>();
            foreach (var contractOrderInstance in employeeOrders)
            {
                if (contractOrderInstance != null)
                {
                    orders.Add(ToDataContractsObject(contractOrderInstance));
                }
            }
            return orders;
        }
        public static Contracts.EmployeeOrderDetails ToDataContractsObject(Entities.EmployeeOrderDetails entityEmployeeOrderInstance)
        {
            Contracts.EmployeeOrderDetails contractEmployeeOrderDetails = new Contracts.EmployeeOrderDetails();
            contractEmployeeOrderDetails.Date = entityEmployeeOrderInstance.Date;
            contractEmployeeOrderDetails.OrderId = entityEmployeeOrderInstance.OrderId;
            if (entityEmployeeOrderInstance.Shelf != null)
            {
                contractEmployeeOrderDetails.Shelf = ShelfTranslator.ToDataContractsObject(entityEmployeeOrderInstance.Shelf);
            }
            if (entityEmployeeOrderInstance.EmployeeItemsQuantityList != null)
            {
                contractEmployeeOrderDetails.EmployeeItemsQuantityList = ShelfItemsTranslator.ToDataContractsObject(entityEmployeeOrderInstance.EmployeeItemsQuantityList);
            }
            return contractEmployeeOrderDetails;
        }
        public static Entities.EmployeeOrder ToEntitiesObject(Contracts.EmployeeOrder employeeOrder)
        {
            Entities.EmployeeOrder entityEmployeeOrder = new Entities.EmployeeOrder();
            if (employeeOrder != null)
            {
                if (employeeOrder.Employee != null)
                {
                    entityEmployeeOrder.Employee = EmployeeTranslator.ToEntitiesObject(employeeOrder.Employee);
                }
                if (employeeOrder.EmployeeOrderDetails != null)
                {
                    entityEmployeeOrder.EmployeeOrderDetails = ToEntitiesObject(employeeOrder.EmployeeOrderDetails);
                }
            }
            return entityEmployeeOrder;
        }
    }
}
