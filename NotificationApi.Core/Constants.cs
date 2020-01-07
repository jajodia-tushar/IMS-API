using System;
using System.Collections.Generic;
using System.Text;

namespace NotificationApi.Core
{
    public static class Constants
    {
        public static class ErrorCodes
        {
            public const int BadRequest = 400;
            public const int UnAuthorized = 401;
            public const int ServerError = 500;
        }
        public static class ErrorMessages
        {
            public const string ServerError = "Internal Server Error";
            public const string InvalidToken = "Invalid Token";
            public const string InvalidEmail = "Invalid Email";
        }
    }
}
