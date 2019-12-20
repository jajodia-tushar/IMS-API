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

            public const string EmptyShelf = "No items are present in the shelf";   

            public const string EmptyShelfList = "Shelf List is not Found";

            public const string InvalidShelfCode = "Invalid Shelf Code";
            public const string ShelfIsAlreadyPresent = "Shelf Is Already Present";


            public const string ShelfNotPresent = "Shelf is not Present";
            public const string UnprocessableEntity = "Data is not valid";
            public const string Conflict = "Item not added";
            public const string InvalidItemsDetails = "Invalid Item Details";
            public const string resourceNotFound = "Items Not Found";

            public const string NotUpdated = "Items Not Updated";
            public const string AlreadyPresent = "Item Already Added";
            public const string AlreadyDeleted = "Item Already Deleted";
            public const string InValidId = "Invalid Id";
            public const string NoOrdersYet = "No orders to display";
            public const string UnableToPlaceOrder = "Unable to place order, check entered fields";
            public const string MissingValues = "Missing entries";
            public const string NoVendorsYet = "No Vendors To Display";

            public const string InvalidToken = "Token is invalid";
            
            public const string LogoutFailed = "Logout Failed";
            public const string OrderNotDeleted = "Vendor Order Not Deleted";
            public const string InvalidOrderId = "Order Id is not Valid";           
            public const string UnableToShowRecentEntries = "Unable to show Entries";
            public const string EmptyRecentEmployeeOrderList = "Recent Entries made by Employee are Empty";
            public const string NoUsers = "No Users Available";
            public const string InvalidInput = "Input details are not valid";
            public const string InvalidDates = "StartDate should be less than EndDate";
            public const string RecordNotFound = "Record Not Found";
            public const string InvalidDate = "Date Is Invalid";
        }

            public const string InvalidToken = "Token is invalid";
            public const string NotDeleted = "Item Not Deleted";
            public const string InvalidItemId = "Item Id is not valid";
        }

        public static class ErrorCodes
        {
            public const int BadRequest = 400;
            public const int UnAuthorized = 401;
            public const int ServerError = 500;
            public const int NotFound = 404;
            public const int UnprocessableEntity = 422;

            public const int Conflict = 409;
            public const int ResourceNotFound = 404;
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
