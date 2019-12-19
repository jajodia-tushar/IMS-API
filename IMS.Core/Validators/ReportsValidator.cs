using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Validators
{
    public class ReportsValidator
    {
        public static MostConsumedItemsResponse ValidateDateAndItemsCount(string startDate, string endDate, int itemsCount)
        {
            MostConsumedItemsResponse mostConsumedItemsResponse = new MostConsumedItemsResponse();
            if (string.IsNullOrEmpty(startDate) || string.IsNullOrEmpty(endDate) || itemsCount <= 0)
            {
                mostConsumedItemsResponse.Status = Status.Failure;
                mostConsumedItemsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidInput);
            }
            if (ValidateDate(startDate, endDate))
            {
                mostConsumedItemsResponse.Status = Status.Success;
            }
            else
            {
                mostConsumedItemsResponse.Status = Status.Failure;
                mostConsumedItemsResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidDate);
            }
            return mostConsumedItemsResponse;
        }
        public static bool ValidateDate(string startDate, string endDate)
        {
            MostConsumedItemsResponse mostConsumedItemsResponse = new MostConsumedItemsResponse();
            int resultantValue = 0;
            DateTime dateValue;
            if (startDate.Length < 8 || endDate.Length < 8)
                return false;
            else if (!Int32.TryParse(startDate, out resultantValue) || !Int32.TryParse(endDate, out resultantValue))
                return false;
            else if (String.Compare(startDate, endDate) > 0)
                return false;
            else
            {
                startDate = startDate.Substring(0, 4) + "/" + startDate.Substring(4, 2) + "/" + startDate.Substring(6, 2);
                endDate = endDate.Substring(0, 4) + "/" + endDate.Substring(4, 2) + "/" + endDate.Substring(6, 2);
                if (!DateTime.TryParse(startDate, out dateValue) || !DateTime.TryParse(endDate, out dateValue))
                    return false;
            }
            return true;
        }
    }
}
