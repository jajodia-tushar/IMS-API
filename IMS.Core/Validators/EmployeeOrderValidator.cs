using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Validators
{
    public class EmployeeOrderValidator
    {
        public static bool ValidateEmployeeOrder(EmployeeOrder employeeOrder)
        {
            if (employeeOrder == null)
            {
                return false;
            }
            if (employeeOrder.Employee == null || string.IsNullOrEmpty(employeeOrder.Employee.Id))
            {
                return false;
            }
            if (employeeOrder.EmployeeOrderDetails == null || employeeOrder.EmployeeOrderDetails.Shelf == null || employeeOrder.EmployeeOrderDetails.Shelf.Id == 0)
            {
                return false;
            }
            if (employeeOrder.EmployeeOrderDetails.EmployeeItemsQuantityList == null || employeeOrder.EmployeeOrderDetails.EmployeeItemsQuantityList.Count == 0)
            {
                return false;
            }
            foreach (ItemQuantityMapping itemQuantityMapping in employeeOrder.EmployeeOrderDetails.EmployeeItemsQuantityList)
            {

                if (itemQuantityMapping == null || itemQuantityMapping.Item == null || itemQuantityMapping.Item.Id == 0 || itemQuantityMapping.Quantity == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
