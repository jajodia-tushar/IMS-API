using IMS.Entities;
using IMS.Entities.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Validators
{
    public class EmployeeBulkOrderValidator
    {
        public static bool Validate(EmployeeBulkOrder employeeBulkOrder)
        {
            if (employeeBulkOrder == null)
                return false;
            var employee = employeeBulkOrder.Employee;
            if (employee == null || string.IsNullOrEmpty(employee.Id))
                return false;
            var orderDetails= employeeBulkOrder.EmployeeBulkOrderDetails;
            if (orderDetails == null || string.IsNullOrEmpty(orderDetails.ReasonForRequirement)|| orderDetails.EmployeeItemsQuantityList == null)
                return false;
            DateTime date = DateTime.Now.AddDays(2);
            DateTime minTime=new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
            if (orderDetails.RequirementDate < minTime)
                throw new InvalidDateFormatException(Constants.ErrorMessages.InvalidBulkRequestDate);
            var itemQuantityList = orderDetails.EmployeeItemsQuantityList;
            if (!Validate(itemQuantityList))
                return false;
            return true; 
        }
        public static bool Validate(List<ItemQuantityMapping> itemQuantityList)
        {
            if (itemQuantityList == null || itemQuantityList.Count == 0)
                return false;
            foreach(var itemQuantityMapping in itemQuantityList)          
                if (!Validate(itemQuantityMapping))
                    return false;          
            return true;
        }
        public static bool Validate(ItemQuantityMapping itemQuantityMapping)
        {
            if (itemQuantityMapping.Item == null || itemQuantityMapping.Item.Id <= 0 || itemQuantityMapping.Quantity <= 0)
                return false;
            return true;
        }
    }
}
