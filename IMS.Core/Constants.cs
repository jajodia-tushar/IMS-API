using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core
{
    public static class Constants
    {
        public static class ErrorMessages
        {
            public const string InvalidUserNameOrPassword = "Invalid Username or Password";

            public const string MissingUsernameOrPassword = "Missing Username or Password";

            public const string ServerError = "Internal Server Error";

            public const string TokenExpired = "Token Expired";

            public const string InvalidId = "Invalid Employee Id";


        }
       
        public static class ErrorCodes
        {
            public const int BadRequest = 400;
            public const int UnAuthorized = 401;
            public const int ServerError = 500;
            public const int NotFound = 404;
        }
       



    }
}
