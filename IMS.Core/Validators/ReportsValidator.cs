using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
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
            DateTime date;
            CultureInfo provider = CultureInfo.InvariantCulture;
            if (String.Compare(startDate, endDate) > 0)
            {
                return false;
            }
            else if(!DateTime.TryParseExact(startDate, "yyyyMMdd", provider, DateTimeStyles.None, out date) || !DateTime.TryParseExact(endDate, "yyyyMMdd", provider, DateTimeStyles.None, out date))
            {
                return false;
            }
            return true;
        }
    }
}
