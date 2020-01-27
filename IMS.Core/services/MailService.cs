using IMS.DataLayer.Interfaces;
using IMS.Entities;
using IMS.Entities.Interfaces;
using IMS.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Core.services
{
    public class MailService : IMailService
    {
        private INotificationProvider _notificationProvider;
        private IEmployeeService _employeeService;
        private ILogManager _logger;
        private IUserDbContext _userDbContext;
        public MailService(INotificationProvider notificationProvider, IEmployeeService employeeService, ILogManager logger,IUserDbContext userDbContext)
        {
            this._notificationProvider = notificationProvider;
            this._employeeService = employeeService;
            this._logger = logger;
            this._userDbContext = userDbContext;

        }
        public async Task<bool> SendEmployeeOrderReciept(EmployeeOrder employeeOrder)
        {
            try
            {
                var email = new Email();
                var employeeResponse = await _employeeService.ValidateEmployee(employeeOrder.Employee.Id);
                if (employeeResponse.Status.Equals(Status.Success))
                {
                    email.ToAddress = employeeResponse.Employee.Email;
                    email.Body = GenerateEmployeeOrderHTMLTemplate(employeeOrder.EmployeeOrderDetails.EmployeeItemsQuantityList, employeeResponse.Employee.Firstname + " " + employeeResponse.Employee.Lastname);
                    email.Subject = "Order Reciept";
                    return await _notificationProvider.SendEmail(email);
                }
                return false;
            }
            catch (Exception exception)
            {
                new Task(() => { _logger.LogException(exception, "SendEmployeeOrderReciept", IMS.Entities.Severity.Medium, employeeOrder, false); }).Start();
                throw exception;
            }
        }
        public async Task<bool> SendEmployeeBulkOrderReciept(EmployeeBulkOrder employeeBulkOrder, BulkOrderRequestStatus orderStatus)
        {
            try
            {
                var email = new Email();
                var employeeResponse = await _employeeService.ValidateEmployee(employeeBulkOrder.Employee.Id);
                if (employeeResponse.Status.Equals(Status.Success))
                {
                    email.ToAddress = employeeResponse.Employee.Email;
                    email.Body = GenerateEmployeeBulkOrderHTMLTemplate(employeeBulkOrder.BulkOrderId,employeeBulkOrder.EmployeeBulkOrderDetails,orderStatus, employeeResponse.Employee.Firstname+" "+employeeResponse.Employee.Lastname);
                    email.Subject = "Order Id:#"+employeeBulkOrder.BulkOrderId+" is "+orderStatus.ToString();
                    return await _notificationProvider.SendEmail(email);
                }
                return false;
            }
            catch (Exception exception)
            {
                new Task(() => { _logger.LogException(exception, "SendEmployeeBulkOrderReciept", IMS.Entities.Severity.Medium, employeeBulkOrder, false); }).Start();
                throw exception;
            }
        }

        private string GenerateEmployeeBulkOrderHTMLTemplate(int orderId ,EmployeeBulkOrderDetails orderDetails,BulkOrderRequestStatus orderStatus,string name)
        {
            string emailBody = @"
                <body style='padding : 0px; margin: 0px;font-family: sans-serif;'>
                    <section style='margin-top: 40px;'>
                        <div class='container' style='width: 80%; margin-left: 10%;'>";
            emailBody += "<h2 style='margin-bottom: 10px;'>Hello, &nbsp" + name + "&nbsp</h2>";
            emailBody += "<h4 style='margin: 0px;'>You have placed bulk order of &nbsp" + orderDetails.ItemsQuantityList.Count + "&nbsp items</h4><br>";
            emailBody += "<h4 style='margin: 0px;'>Order Status:&nbsp" +orderStatus.ToString() + "&nbsp</h4><br>";
            emailBody += "<h4 style='margin: 0px;'>Reason For Requirement :&nbsp" + orderDetails.ReasonForRequirement + "&nbsp</h4><br>";
            emailBody += "<h4 style='margin: 0px;'>Requirement Date :&nbsp" + orderDetails.RequirementDate.ToString() + "&nbsp</h4><br>";
            emailBody += "<h3 style='margin: 0px;'>Item Details&nbsp</h4>";

            emailBody += @"<div class='item-table' style='margin-top: 40px;'>
                                <table style='border-collapse: collapse; border: 1px solid #d4c7c7; width: 100%;'>
                                    <tr>
                                        <th style='border: 1px solid #d4c7c7; padding: 15px;'>Item name</th>
                                        <th style='border: 1px solid #d4c7c7; padding: 15px;'>Quantity Ordered</th>
                                    </tr>";
            foreach (var employeeItemQuantity in orderDetails.ItemsQuantityList)
            {
                emailBody += ("<tr><td style='border: 1px solid #d4c7c7; padding: 15px; text-align: center;'>" + employeeItemQuantity.Item.Name + "</td>");
                emailBody += ("<td style='border: 1px solid #d4c7c7; padding: 15px;text-align: center;'>" + employeeItemQuantity.QuantityOrdered + "</td></tr>");
            }

            emailBody += @"</table>
                            </div>
                            <div class='report-text' style='margin-top: 30px; margin-bottom: 30px;'>
                                <h5>Please drop an email to admin@tavisca.com to report if this transaction was not authorized by you.</h5>
                            </div>
                            <div class='regards-text' style='margin-bottom: 50px;'>
                                <h5>Regards, <br> Tavisca Admin Team</h5>
                            </div>
                        </div>
                    </section>";

            emailBody += @"<footer style='position: fixed; bottom: 0;color: #fff; background-color: #222437; width: 100%; text-align: center; height: 30px;'> 
                            <div class='footer-text' style='padding-top: 9px; font-size: small;'>";

            emailBody += "&copy; Tavisca Solutions Pvt. Ltd " + DateTime.Today.ToString("yyyy") + " All Rights Reserved";

            emailBody += @"</div>
                    </footer>
               </body>";

            return emailBody;
        }


        public string GenerateEmployeeOrderHTMLTemplate(List<ItemQuantityMapping> employeeItemsQuantityList, string name)
        {
            string emailBody = @"
                <body style='padding : 0px; margin: 0px;font-family: sans-serif;'>
                    <section style='margin-top: 40px;'>
                        <div class='container' style='width: 80%; margin-left: 10%;'>";
            emailBody += "<h2 style='margin-bottom: 10px;'>Hello, &nbsp" + name + "&nbsp</h2>";
            emailBody += "<h4 style='margin: 0px;'>You have taken &nbsp" + employeeItemsQuantityList.Count + "&nbsp items</h4>";
                         
            emailBody += @"<div class='item-table' style='margin-top: 40px;'>
                                <table style='border-collapse: collapse; border: 1px solid #d4c7c7; width: 100%;'>
                                    <tr>
                                        <th style='border: 1px solid #d4c7c7; padding: 15px;'>Item name</th>
                                        <th style='border: 1px solid #d4c7c7; padding: 15px;'>Quantity</th>
                                    </tr>";
            foreach (var employeeItemQuantity in employeeItemsQuantityList)
            {
                emailBody += ("<tr><td style='border: 1px solid #d4c7c7; padding: 15px; text-align: center;'>" + employeeItemQuantity.Item.Name + "</td>");
                emailBody += ("<td style='border: 1px solid #d4c7c7; padding: 15px;text-align: center;'>" + employeeItemQuantity.Quantity + "</td></tr>");
            }

            emailBody += @"</table>
                            </div>
                            <div class='report-text' style='margin-top: 30px; margin-bottom: 30px;'>
                                <h5>Please drop an email to admin@tavisca.com to report if this transaction was not authorized by you.</h5>
                            </div>
                            <div class='regards-text' style='margin-bottom: 50px;'>
                                <h5>Regards, <br> Tavisca Admin Team</h5>
                            </div>
                        </div>
                    </section>";

            emailBody += @"<footer style='position: fixed; bottom: 0;color: #fff; background-color: #222437; width: 100%; text-align: center; height: 30px;'> 
                            <div class='footer-text' style='padding-top: 9px; font-size: small;'>";

            emailBody += "&copy; Tavisca Solutions Pvt. Ltd " + DateTime.Today.ToString("yyyy") + " All Rights Reserved";

            emailBody += @"</div>
                    </footer>
               </body>";

            return emailBody;
        }

        public async Task<bool> SendApprovedBulkOrderToLoggedInUser(int loggedInUserId,EmployeeBulkOrder order, ApproveEmployeeBulkOrder approveEmployeeBulkOrder)
        {
            try
            {
                var email = new Email();
                var user = await _userDbContext.GetUserById(loggedInUserId);
                if (user!=null)
                {
                    email.ToAddress = "preddy@tavisca.com";
                    email.Body = GenerateApprovedBulkOrderHTMLTemplateForAdmin(user,order, approveEmployeeBulkOrder.ItemLocationQuantityMappings,order.Employee);
                    email.Subject = "Approved Order Id#:"+order.BulkOrderId+" "+"Receipt";
                    return await _notificationProvider.SendEmail(email);
                }
                return false;
            }
            catch (Exception exception)
            {
                new Task(() => { _logger.LogException(exception, "SendApprovedBulkOrderToLoggedInUser", IMS.Entities.Severity.Critical, approveEmployeeBulkOrder, false); }).Start();
                throw exception;
            }
        }

        private string GenerateApprovedBulkOrderHTMLTemplateForAdmin(User LoggedInUser,EmployeeBulkOrder order, List<ItemLocationQuantityMapping> itemLocationQuantityMapping, Employee employee)
        {
            Dictionary<int, List<LocationQuantityMapping>> getLocationQuantity = new Dictionary<int, List<LocationQuantityMapping>>();
            foreach(ItemLocationQuantityMapping itemlocationQuantity in itemLocationQuantityMapping)
            {
                getLocationQuantity.Add(itemlocationQuantity.Item.Id, itemlocationQuantity.LocationQuantityMappings);
            }
            string emailBody = @"
                <body style='padding : 0px; margin: 0px;font-family: sans-serif;'>
                    <section style='margin-top: 40px;'>
                        <div class='container' style='width: 80%; margin-left: 10%;'>";
            emailBody += "<h2 style='margin-bottom: 10px;'>Hello, &nbsp" + LoggedInUser.Firstname+" "+LoggedInUser.Lastname + "&nbsp</h2>";
            emailBody += "<h3 style='margin: 0px;'>You have Approved bulkorder of Id:# " + order.BulkOrderId + "&nbsp</h3><br>";
            emailBody += "<h3 style='margin: 0px;'>Order Details &nbsp</h3><br>";
            emailBody += "<h5 style='margin: 0px;'>EmployeeId: &nbsp" + employee.Id+ "&nbsp</h5><br>";
            emailBody += "<h5 style='margin: 0px;'>Employee Name: &nbsp" + employee.Firstname+" "+employee.Lastname + "&nbsp</h5><br>";
            emailBody += "<h5 style='margin: 0px;'>Employee EmaildId: &nbsp" + employee.Email + "&nbsp</h5><br>";
            emailBody += "<h5 style='margin: 0px;'>Reason for Requirement: &nbsp" + order.EmployeeBulkOrderDetails.ReasonForRequirement + "&nbsp</h5><br>";
            emailBody += "<h5 style='margin: 0px;'>RequirementDate: &nbsp" + order.EmployeeBulkOrderDetails.ReasonForRequirement.ToString() + "&nbsp</h5><br>";
            emailBody += "<h3 style='margin: 0px;'>Item Details &nbsp</h3>";
            emailBody += @"<div class='item-table' style='margin-top: 40px;'>
                                <table style='border-collapse: collapse; border: 1px solid #d4c7c7; width: 100%;'>
                                    <tr>
                                        <th style='border: 1px solid #d4c7c7; padding: 15px;'>Item name</th>
                                        <th style='border: 1px solid #d4c7c7; padding: 15px;'>Total Quantity</th>
                                        <th style='border: 1px solid #d4c7c7; padding: 15px;'>Quantity Assigned To Location</th>
                                    </tr>";
            foreach (BulkOrderItemQuantityMapping itemQunatity in order.EmployeeBulkOrderDetails.ItemsQuantityList)
            {
                emailBody += ("<tr><td style='border: 1px solid #d4c7c7; padding: 15px; text-align: center;'>" + itemQunatity.Item.Name + "</td>");
                emailBody += ("<td style='border: 1px solid #d4c7c7; padding: 15px;text-align: center;'>" + itemQunatity.QuantityOrdered + "</td>");
                string locationQuantity = "";
                List<LocationQuantityMapping> locationQuantityMappings = getLocationQuantity[itemQunatity.Item.Id];
                foreach (LocationQuantityMapping locationQuantityMapping in locationQuantityMappings)
                    locationQuantity += locationQuantityMapping.Location + ":" + locationQuantityMapping.Quantity + "<br>";
                emailBody += ("<td style='border: 1px solid #d4c7c7; padding: 15px;text-align: center;'>"+ locationQuantity+"</td></tr>");

            }

            emailBody += @"</table>
                            </div>
                            <div class='report-text' style='margin-top: 30px; margin-bottom: 30px;'>
                                <h5>Please drop an email to admin@tavisca.com to report if this transaction was not authorized by you.</h5>
                            </div>
                            <div class='regards-text' style='margin-bottom: 50px;'>
                                <h5>Regards, <br> Tavisca IMS </h5>
                            </div>
                        </div>
                    </section>";

            emailBody += @"<footer style='position: fixed; bottom: 0;color: #fff; background-color: #222437; width: 100%; text-align: center; height: 30px;'> 
                            <div class='footer-text' style='padding-top: 9px; font-size: small;'>";

            emailBody += "&copy; Tavisca Solutions Pvt. Ltd " + DateTime.Today.ToString("yyyy") + " All Rights Reserved";

            emailBody += @"</div>
                    </footer>
               </body>";

            return emailBody;
        }
    }
}