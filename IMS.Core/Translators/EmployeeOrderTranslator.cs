using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public class EmployeeOrderTranslator
    {
        public static Contracts.EmployeeRecentOrderResponse ToDataContractsObject(Entities.EmployeeRecentOrderResponse doEmployeeRecentOrderResponse)
        {
            Contracts.EmployeeRecentOrderResponse dtoEmployeeRecentOrderResponse = new Contracts.EmployeeRecentOrderResponse();
            dtoEmployeeRecentOrderResponse.Status = Contracts.Status.Failure;
            if (doEmployeeRecentOrderResponse.Status == Entities.Status.Success)
            {
                dtoEmployeeRecentOrderResponse.Status = Contracts.Status.Success;
                dtoEmployeeRecentOrderResponse.Error = null;
                dtoEmployeeRecentOrderResponse.EmployeeRecentOrders = ToDataContractsObject(doEmployeeRecentOrderResponse.EmployeeRecentOrders);
            }
            else
            {
                dtoEmployeeRecentOrderResponse.Status = Contracts.Status.Failure;
                dtoEmployeeRecentOrderResponse.Error = Translator.ToDataContractsObject(doEmployeeRecentOrderResponse.Error);
            }

            return dtoEmployeeRecentOrderResponse;
        }

        public static List<Contracts.EmployeeRecentOrder> ToDataContractsObject(List<Entities.EmployeeRecentOrder> employeeRecentOrders)
        {
            List<Contracts.EmployeeRecentOrder> dtoEmployeeRecentOrder = new List<Contracts.EmployeeRecentOrder>();
            foreach (var recentOrder in employeeRecentOrders)
            {
                dtoEmployeeRecentOrder.Add(ToDataContractsObject(recentOrder));
            }

            return dtoEmployeeRecentOrder;
        }

        public static Contracts.EmployeeRecentOrder ToDataContractsObject(Entities.EmployeeRecentOrder recentOrder)
        {
            Contracts.EmployeeRecentOrder dtoEmployeeRecentOrder = new Contracts.EmployeeRecentOrder();
            dtoEmployeeRecentOrder.Employee = EmployeeTranslator.ToDataContractsObject(recentOrder.Employee);
            dtoEmployeeRecentOrder.EmployeeOrder = ToDataContractsObject(recentOrder.EmployeeOrder);
            return dtoEmployeeRecentOrder;
        }

        public static Entities.EmployeeOrderDetails ToEntitiesObject(Contracts.EmployeeOrderDetails employeeOrder)
        {
            return new Entities.EmployeeOrderDetails()
            {
                Date = employeeOrder.Date,
                OrderId = employeeOrder.OrderId,
                Shelf = ShelfTranslator.ToEntitiesObject(employeeOrder.Shelf),
                EmployeeItemsQuantityList = ShelfItemsTranslator.ToEntitiesObject(employeeOrder.EmployeeItemsQuantityList)
            };
        }

        public static Contracts.EmployeeOrderResponse ToDataContractsObject(Entities.EmployeeOrderResponse placeEmployeeOrderResponseEntity)
        {
            Contracts.EmployeeOrderResponse contractPlaceEmployeeOrderResponse = new Contracts.EmployeeOrderResponse();
            contractPlaceEmployeeOrderResponse.Status = Contracts.Status.Failure;
            contractPlaceEmployeeOrderResponse.Error = Translator.ToDataContractsObject(placeEmployeeOrderResponseEntity.Error);
            if (placeEmployeeOrderResponseEntity.Status == Entities.Status.Success)
            {
                contractPlaceEmployeeOrderResponse.Status = Contracts.Status.Success;
                contractPlaceEmployeeOrderResponse.Error = null;
                contractPlaceEmployeeOrderResponse.EmployeeOrder = ToDataContractsObject(placeEmployeeOrderResponseEntity.EmployeeOrder);
            }
            return contractPlaceEmployeeOrderResponse;
        }

        private static Contracts.EmployeeOrder ToDataContractsObject(Entities.EmployeeOrder employeeOrderEntity)
        {
            return new Contracts.EmployeeOrder()
            {
                Employee = EmployeeTranslator.ToDataContractsObject(employeeOrderEntity.Employee),
                EmployeeOrderDetails = ToDataContractsObject(employeeOrderEntity.EmployeeOrderDetails)
            };
        }

        public static Contracts.OrdersByEmployeeIdResponse ToDataContractsObject(Entities.OrdersByEmployeeIdResponse entityEmployeeOrderResponse)
        {
            Contracts.OrdersByEmployeeIdResponse employeeOrderResponse = new Contracts.OrdersByEmployeeIdResponse();
            employeeOrderResponse.Status = Contracts.Status.Failure;
            employeeOrderResponse.Error = Translator.ToDataContractsObject(entityEmployeeOrderResponse.Error);
            if (entityEmployeeOrderResponse.Status == Entities.Status.Success)
            {
                employeeOrderResponse.Status = Contracts.Status.Success;
                employeeOrderResponse.Error = null;
                employeeOrderResponse.EmployeeOrders = ToDataContractsObject(entityEmployeeOrderResponse.EmployeeOrders);
            }
            if (entityEmployeeOrderResponse.Employee != null)
                employeeOrderResponse.Employee = EmployeeTranslator.ToDataContractsObject(entityEmployeeOrderResponse.Employee);
            return employeeOrderResponse;
        }
        public static List<Contracts.EmployeeOrderDetails> ToDataContractsObject(List<Entities.EmployeeOrderDetails> employeOrders)
        {
            List<Contracts.EmployeeOrderDetails> orders = new List<Contracts.EmployeeOrderDetails>();
            foreach (var contractOrderInstance in employeOrders)
            {
                orders.Add(ToDataContractsObject(contractOrderInstance));
            }
            return orders;
        }


        public static Contracts.EmployeeOrderDetails ToDataContractsObject(Entities.EmployeeOrderDetails entityEmployeeOrderInstance)
        {
            return new IMS.Contracts.EmployeeOrderDetails()
            {
                Date = entityEmployeeOrderInstance.Date,
                OrderId = entityEmployeeOrderInstance.OrderId,
                Shelf = ShelfTranslator.ToDataContractsObject(entityEmployeeOrderInstance.Shelf),
                EmployeeItemsQuantityList = ShelfItemsTranslator.ToDataContractsObject(entityEmployeeOrderInstance.EmployeeItemsQuantityList)
            };
        }

        public static Entities.EmployeeOrder ToEntitiesObject(Contracts.EmployeeOrder employeeOrder)
        {
            return new Entities.EmployeeOrder()
            {
                Employee = EmployeeTranslator.ToEntitiesObject(employeeOrder.Employee),
                EmployeeOrderDetails = ToEntitiesObject(employeeOrder.EmployeeOrderDetails)
            };
        }
    }
}
