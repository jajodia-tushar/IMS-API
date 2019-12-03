using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Logging
{
    public class LogConstants
    {
        public static class Status
        {
            public const string Success = "Success";
            public const string Failure = "Failure";
            public const string No = "No";

        }
        public static class Severity
        {
            public const string Critical = "Critical";
            public const string High = "High";
            public const string Medium = "Medium";
            public const string Low = "Low";
            public const string No = "No";

        }
        public static class CallTypes
        {
            public const string Login = "Login";

            public static Dictionary<string, string> SeverityMapping = new Dictionary<string, string>
            {
                {CallTypes.Login,Severity.Critical }

            };
        }
    }

}
