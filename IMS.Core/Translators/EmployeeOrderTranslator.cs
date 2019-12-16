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

    }
}
