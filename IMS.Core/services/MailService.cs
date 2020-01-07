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
            string emailBody = "<div style = 'float:left;'><h3>Hi&nbsp" + name + "&nbsp</h3>";
            emailBody += "You have taken &nbsp"+employeeItemsQuantityList.Count+"&nbsp Items<br><br>";
            emailBody += "<table>";
            emailBody+= "<tr><th>Item Name</th><th>Quantity</th></tr>";
            foreach(var employeeItemQuantity in employeeItemsQuantityList)
            {
                emailBody += ("<tr><td>" + employeeItemQuantity.Item.Name + "</td>");
                emailBody+= ("<td>" + employeeItemQuantity.Quantity + "</td></tr>");
            }
            emailBody += "</table>";
            emailBody += "<br>Please drop an email to mnaukarkar@tavisca.com to report if this transaction was not authorized by you<br>";
            emailBody += "<br>Regards,<br>Tavisca Admin Team<br></div>";
            return emailBody;
        }
    }
}