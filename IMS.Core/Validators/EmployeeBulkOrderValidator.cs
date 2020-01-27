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
            if (orderFromRequest == null)
                return false;
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
        
        public static bool isEmployeeBulkOrderReturnValid(EmployeeBulkOrder employeeBulkOrder, EmployeeBulkOrder dbEmployeeBulkOrder)
        {
            return (IsQuantityUsedLessThanOrdered(employeeBulkOrder) &&
                IsOrderUsedQuantityNonNegative(employeeBulkOrder) &&
            IsQuantityUsedLessThanPreviousQuantityUsed(employeeBulkOrder,dbEmployeeBulkOrder) &&
            IsOrderStatusValid(employeeBulkOrder,dbEmployeeBulkOrder)&&
            IsItemCountEqual(employeeBulkOrder.EmployeeBulkOrderDetails.ItemsQuantityList, dbEmployeeBulkOrder.EmployeeBulkOrderDetails.ItemsQuantityList));
        }

        private static bool IsOrderUsedQuantityNonNegative(EmployeeBulkOrder employeeBulkOrder)
        {
            foreach(var item in employeeBulkOrder.EmployeeBulkOrderDetails.ItemsQuantityList)
            {
                if (item.QuantityUsed < 0)
                    return false;
            }
            return true;
        }

        private static bool IsOrderStatusValid(EmployeeBulkOrder employeeBulkOrder, EmployeeBulkOrder dbEmployeeBulkOrder)
        {
            if (employeeBulkOrder.EmployeeBulkOrderDetails.BulkOrderRequestStatus == Entities.BulkOrderRequestStatus.Approved &&
                employeeBulkOrder.EmployeeBulkOrderDetails.BulkOrderRequestStatus == dbEmployeeBulkOrder.EmployeeBulkOrderDetails.BulkOrderRequestStatus)
                return true;
            return false;
        }

        private static bool IsItemCountEqual(List<BulkOrderItemQuantityMapping> previousItemsQuantityList, List<BulkOrderItemQuantityMapping> newItemsQuantityList)
        {
            if (previousItemsQuantityList.Count != newItemsQuantityList.Count)
                return false;
            return true;
        }

        private static bool IsQuantityUsedLessThanPreviousQuantityUsed(EmployeeBulkOrder employeeBulkOrder, EmployeeBulkOrder DbEmployeeBulkOrder)
        {
            List<BulkOrderItemQuantityMapping> previousItemMapping = employeeBulkOrder.EmployeeBulkOrderDetails.ItemsQuantityList;
            List<BulkOrderItemQuantityMapping> newItemMapping = DbEmployeeBulkOrder.EmployeeBulkOrderDetails.ItemsQuantityList;
            for (int index=0; index<previousItemMapping.Count; index++)
            {
                if (previousItemMapping[index].QuantityUsed < newItemMapping[index].QuantityUsed)
                    return false;
            }
            return true;
        }

        private static Boolean IsQuantityUsedLessThanOrdered(EmployeeBulkOrder employeeBulkOrder)
        {
            foreach (var item in employeeBulkOrder.EmployeeBulkOrderDetails.ItemsQuantityList)
            {
                if (item.QuantityOrdered < item.QuantityUsed)
                    return false;
            }
            return true;
        }
    }
}
