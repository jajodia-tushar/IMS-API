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
            if (orderDetails == null || string.IsNullOrEmpty(orderDetails.ReasonForRequirement)|| orderDetails.ItemsQuantityList == null)
                return false;
            DateTime date = DateTime.Now.AddDays(2);
            DateTime minTime=new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
            if (orderDetails.RequirementDate < minTime)
                throw new InvalidDateFormatException(Constants.ErrorMessages.InvalidBulkRequestDate);
            var itemQuantityList = orderDetails.ItemsQuantityList;
            if (!ValidatePlacedItems(itemQuantityList))
                return false;
            return true; 
        }
        public static bool ValidatePlacedItems(List<BulkOrderItemQuantityMapping> itemQuantityList)
        {
            if (itemQuantityList == null || itemQuantityList.Count == 0)
                return false;
            foreach(var itemQuantityMapping in itemQuantityList)          
                if (!ValidatePlacedItem(itemQuantityMapping))
                    return false;          
            return true;
        }
        public static bool ValidatePlacedItem(BulkOrderItemQuantityMapping itemQuantityMapping)
        {
            if (itemQuantityMapping.Item == null || itemQuantityMapping.Item.Id <= 0 || itemQuantityMapping.QuantityOrdered <= 0)
                return false;
            return true;
        }

        public static bool ValidateToApprove(ApproveEmployeeBulkOrder orderFromRequest, EmployeeBulkOrder orderFromDatabase)
        {
            if (orderFromDatabase.BulkOrderId != orderFromRequest.BulkOrderId)
                return false;
            if (orderFromRequest == null||orderFromRequest.ItemLocationQuantityMappings==null)
                return false;
            if (orderFromRequest.ItemLocationQuantityMappings.Count != orderFromDatabase.EmployeeBulkOrderDetails.ItemsQuantityList.Count)
                return false;
            //Required To check whether assigned quntity equal to ordered quntity or not
             Dictionary<int, int> itemIdQuantityFromDB = new Dictionary<int, int>();
            foreach(BulkOrderItemQuantityMapping itemQuantityMapping in orderFromDatabase.EmployeeBulkOrderDetails.ItemsQuantityList)
            {
                itemIdQuantityFromDB.Add(itemQuantityMapping.Item.Id, itemQuantityMapping.QuantityOrdered);
            }
            List<ItemLocationQuantityMapping> listOfItemLocationQuantityMappings= orderFromRequest.ItemLocationQuantityMappings;
            foreach(ItemLocationQuantityMapping itemLocationQuantityMapping in listOfItemLocationQuantityMappings)
            {
                int quantityAsignedForItemFromRequest = 0;
                int itemIdFromRequest = itemLocationQuantityMapping.Item.Id;
                if (itemLocationQuantityMapping.LocationQuantityMappings == null)
                    return false;
                foreach(LocationQuantityMapping locationQuantityMapping in itemLocationQuantityMapping.LocationQuantityMappings)
                {
                    if (string.IsNullOrEmpty(locationQuantityMapping.Location))
                        return false;
                    quantityAsignedForItemFromRequest += locationQuantityMapping.Quantity;
                }
                if (!itemIdQuantityFromDB.ContainsKey(itemIdFromRequest))
                    return false;
                if (itemIdQuantityFromDB[itemIdFromRequest] != quantityAsignedForItemFromRequest)
                    return false;               
            }
            return true;


        }
    }
}
