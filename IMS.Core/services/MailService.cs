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
        public MailService(INotificationProvider notificationProvider, IEmployeeService employeeService, ILogManager logger)
        {
            this._notificationProvider = notificationProvider;
            this._employeeService = employeeService;
            this._logger = logger;

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
    }
}