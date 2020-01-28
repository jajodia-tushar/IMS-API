using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IMS.Core.Validators
{
    public class DateTimeValidator
    {
        public static bool InitializeAndValidateDates(string fromDate, string toDate, out DateTime startDate, out DateTime endDate)
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
