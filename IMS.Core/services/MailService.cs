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

        private string GenerateEmployeeOrderHTMLTemplate(List<ItemQuantityMapping> employeeItemsQuantityList, string name)
        {
            string emailBody = "<div style='font-family:sans-serif;'>";
            emailBody += "<h3 style='color:#244061;'>Hi,&nbsp" + name + "&nbsp</h3>";
            emailBody += "<p style='color:#244061;font-size: 12.0pt;'>You have taken &nbsp" + employeeItemsQuantityList.Count+"&nbsp Items</p>";
            emailBody += "<table><tr><th align='left'>Item Name</th><th align='center'>Quantity</th></tr>";
            foreach(var employeeItemQuantity in employeeItemsQuantityList)
            {
                emailBody += ("<tr><td>" + employeeItemQuantity.Item.Name + "</td>");
                emailBody+= ("<td align='center'>" + employeeItemQuantity.Quantity + "</td></tr>");
            }
            emailBody += "</table>";
            emailBody += "<br>Please drop an email to mnaukarkar@tavisca.com to report if this transaction was not authorized by you<br><br>";
            emailBody += "<div style='color:#244061;font-size: 12.0pt;'>Regards,</div>";
            emailBody += "<div style = 'color:#002060;font-size: 12.0pt;'> Tavisca Admin Team</div>";
            return emailBody;
        }
    }
}