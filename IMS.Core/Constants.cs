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


            public const string EmptyShelfList = "Shelf List is not Found";

            public const string InvalidShelfCode = "Invalid Shelf Code";
            public const string ShelfIsAlreadyPresent = "Shelf Is Already Present";
        }
       
        public static class ErrorCodes
        {
            public const int BadRequest = 400;
            public const int UnAuthorized = 401;
            public const int ServerError = 500;
            public const int NotFound = 404;
        }
        public static class Roles
        {
            public const string SuperAdmin = "superadmin";
            public const string Admin = "admin";
            public const string Shelf = "shelf";
            public const string Clerk = "clerk";
            public static Dictionary<string, int> ExpirationTimeInMinutes = new Dictionary<string, int>()
            {
                { Roles.SuperAdmin,30 },
                { Roles.Admin,30 },
                { Roles.Clerk,60 },
                {Roles.Shelf,600 }
            };
        }



    }
}
