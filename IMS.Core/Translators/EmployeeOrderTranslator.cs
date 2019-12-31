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
            if (doEmployeeRecentOrderResponse != null)
                return new Contracts.EmployeeRecentOrderResponse
                {
                    Status = doEmployeeRecentOrderResponse.Status == Entities.Status.Success ? 
                    Contracts.Status.Success : Contracts.Status.Failure,
                    Error = doEmployeeRecentOrderResponse.Error == null ?
                    null : Translator.ToDataContractsObject(doEmployeeRecentOrderResponse.Error),
                    EmployeeRecentOrders = doEmployeeRecentOrderResponse.EmployeeRecentOrders == null ? 
                    null : ToDataContractsObject(doEmployeeRecentOrderResponse.EmployeeRecentOrders),
                    PagingInfo = doEmployeeRecentOrderResponse.PagingInfo == null ? 
                    null : ToDataContractsObject(doEmployeeRecentOrderResponse.PagingInfo)
                };
            return null;
        }
        public static Contracts.PagingInfo ToDataContractsObject(Entities.PagingInfo pagingInfo)
        {
            if(pagingInfo !=null)
            return new Contracts.PagingInfo()
            {
                PageNumber = pagingInfo.PageNumber,
                PageSize = pagingInfo.PageSize,
                TotalResults = pagingInfo.TotalResults
            };
            return null;
        }
        public static List<Contracts.EmployeeRecentOrder> ToDataContractsObject(List<Entities.EmployeeRecentOrder> employeeRecentOrders)
        {
            List<Contracts.EmployeeRecentOrder> dtoEmployeeRecentOrder = null;
            if (employeeRecentOrders != null)
            { 
              dtoEmployeeRecentOrder = new List<Contracts.EmployeeRecentOrder>();
                foreach (var recentOrder in employeeRecentOrders)
                {
                    dtoEmployeeRecentOrder.Add(ToDataContractsObject(recentOrder));
                }
                return dtoEmployeeRecentOrder;
            }
            return dtoEmployeeRecentOrder;
        }

        public static Contracts.EmployeeRecentOrder ToDataContractsObject(Entities.EmployeeRecentOrder recentOrder)
        {
            if (recentOrder != null)
            {
                return new Contracts.EmployeeRecentOrder
                {
                    Employee = recentOrder.Employee == null ? null : EmployeeTranslator.ToDataContractsObject(recentOrder.Employee),
                    EmployeeOrder = recentOrder.EmployeeOrder == null ? null : ToDataContractsObject(recentOrder.EmployeeOrder)
                };
            }
            return null;
        }

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
                    EmployeeOrder = placeEmployeeOrderResponseEntity.EmployeeOrder==null?null : 
                    ToDataContractsObject(placeEmployeeOrderResponseEntity.EmployeeOrder)
                };
            }
            return null;
        }

        private static Contracts.EmployeeOrder ToDataContractsObject(Entities.EmployeeOrder employeeOrderEntity)
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
        public static Contracts.OrdersByEmployeeIdResponse ToDataContractsObject(Entities.OrdersByEmployeeIdResponse entityEmployeeOrderResponse)
        {
            Contracts.OrdersByEmployeeIdResponse employeeOrderResponse = new Contracts.OrdersByEmployeeIdResponse();
            employeeOrderResponse.Status = Contracts.Status.Failure;
            employeeOrderResponse.Error = Translator.ToDataContractsObject(entityEmployeeOrderResponse.Error);
            if (entityEmployeeOrderResponse.Status == Entities.Status.Success)
            {
                employeeOrderResponse.Status = Contracts.Status.Success;
                employeeOrderResponse.Error = null;
                if (employeeOrderResponse.EmployeeOrders != null && employeeOrderResponse.EmployeeOrders.Count > 0)
                {
                    employeeOrderResponse.EmployeeOrders = ToDataContractsObject(entityEmployeeOrderResponse.EmployeeOrders);
                }
                employeeOrderResponse.EmployeeOrders = ToDataContractsObject(entityEmployeeOrderResponse.EmployeeOrders);
            }
            if (entityEmployeeOrderResponse.Employee != null)
                employeeOrderResponse.Employee = EmployeeTranslator.ToDataContractsObject(entityEmployeeOrderResponse.Employee);
            return employeeOrderResponse;
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
