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
            public const string InvalidOrderReturnDetails = "Invalid order return details were supplied";
            public const string OrderIdDoesNotMatch = "OrderId in payload Is Different";
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

            public const string NoShelfWiseOrderCount = "No Shelf Wise Order Count is Available";
            public const string DateRangeIsInvalid = "Given Date Range Is Invalid";
            public const string NotDeleted = "Item Not Deleted";
            public const string InvalidItemId = "Item Id is not valid";
            public const string UnableToFetch = "unable to fetch from database";
            public const string InvalidRequest = "Invalid Request";
            public const string InvalidOrder = "Invalid Order";
            public const string NoToken = "Token Required";
            public const string TranferFailure = "The items transfer failed";
            public const string DateFormatInvalid = "Given Date Format is Invalid";
            public const string NoItemsInStore = "No items in store";
            public const string UnableToShowStockStatus = "Sorry, Unable to show stock status";
            public const string LocationNotfound = "No Location found on given Input";
            public const string AlreadyEdited = "Cannot Edit Order Again";
            public const string EmailFormatError = "Invalid Email Format";
            public const string EmptyLogList = "No Log history found ";
            public const string UserDetailsMissing = "Fields Cannot be Empty";
            public const string UserNameSpacesError = "No Spaces allowed in UserName";
            public const string UserNameMinMaxLengthError = "UserName should be minimum of 6 and maximum of 15 characters";
            public const string PasswordLowerCaseError = "Password should contain At least one lower case letter";
            public const string PasswordUpperCaseError = "Password should contain At least one upper case letter";
            public const string PasswordLengthError = "Password should be minimum of 8 and maximum of 15 characters";
            public const string PasswordNumberError = "Password should contain At least one numeric value";
            public const string PasswordSymbolError = "Password should contain At least one special case characters";
            public const string PasswordSpaceError = "Password should not contain spaces";
            public const string NoUserFoundToDelete = "No Users found to delete";
            public const string UnAuthorized = "You are not authorized to perform this action";
            public const string InvalidUsername = "Invalid Username";
            public const string InvalidPageNumber = "Page Number is Invalid";
            public const string InvalidEmailId = "Invalid Email Id";
            public const string InvalidPagingDetails = "The page number or page size is invalid";
            public const string DataAlreadyPresent = "Data is already present";
            public const string NoEmployeesPresent = "NoEmployeesPresent";
            public const string EmployeesDataNotFound = "Employees Data Not Found";
            public const string InvalidEmployeeDetails = "Invalid Employee Details";
            public const string AlreadyPresentEmployee = "Employee Id or EmailId Is Already Present";
            public const string EmployeeNotAdded = "Employee Not Added";
            public const string InvalidEmployeeId = "Employee Id is not valid";
            public const string EmployeeNotDeleted = "employee not deleted";

            public const string UserNotFound = "User Not Found";
            public const string NewPasswordRepeated = "New Password Should Not Equal To Old Password";
            public const string PasswordNotUpdated = "Password Not Updated";
            public const string InvalidBulkRequestDate = "Order must be placed atleast Two days before Requirement Date.Please Contact Admin Team for further assistance";
            public const string OrderNotFound = "Order not found for this Id";
            public const string NoNotification = "Notifications not found";
            public const string InvalidOrderToApprove = "Order should  be in Pending state to Approve";
            public const string InvalidOrderStatus = "Order Status Not appropriate for this action";
            public const string ItemsUnavailability = "Items are Unavailable To Approve";
            public const string InvalidOrderToReject = "Order should  be in Pending state to Reject";
            public const string IncorrectOldPassword = "Old Password is Incorrect";

            public const string InvalidShelfDetails = "Shelf Code Or Shelf Name Is Empty";
            public const string ShelfCodeAlreadyPresent = "Shelf Code Already Present";
            public const string ShelfNotUpdated = "Shelf Details Not Updated";

            public const string ActivityLogsNotPresent = "No Activity Logs Present";
        }
        public static class Email
        {
            public const string NoReply = "noreply@tavisca.com";
            public const string TaviscaIMS="Tavisca-IMS@tavisca.com";
            public const string Admin = "admin@tavisca.com";
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
                {Roles.Shelf,720 }
            };
        }



    }
}
