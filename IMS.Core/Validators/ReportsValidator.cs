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
        public static bool IsDateValid(string fromDate, string toDate, out DateTime startDate, out DateTime endDate)
        {
            startDate = DateTime.ParseExact(fromDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            endDate = DateTime.ParseExact(toDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            return IsDateRangeIsValid(startDate, endDate) ? true : false;
        }
        public static bool IsDateRangeIsValid(DateTime startDate, DateTime endDate)
        {
            int value = DateTime.Compare(startDate, endDate);
            if (startDate > DateTime.Now || endDate >= DateTime.Now)
            {
                return false;
            }
            else if (value <= 0)
            {
                return true;
            }
            return false;
        }

        public static bool FilterAndValidateDates(string fromDate, string toDate, out DateTime startDate, out DateTime endDate)
        {
            startDate = new DateTime();
            endDate = new DateTime();
            CultureInfo provider = CultureInfo.InvariantCulture;
            if (String.IsNullOrEmpty(fromDate) && String.IsNullOrEmpty(toDate))
            {
                startDate = DateTime.MinValue;
                endDate = DateTime.Now;
                return true;
            }
            if (String.Compare(fromDate, toDate) > 0)
            {
                return false;
            }
            return (DateTime.TryParseExact(fromDate, "yyyyMMdd", provider, DateTimeStyles.None, out startDate) && DateTime.TryParseExact(toDate, "yyyyMMdd", provider, DateTimeStyles.None, out endDate));
        }
    }
}
