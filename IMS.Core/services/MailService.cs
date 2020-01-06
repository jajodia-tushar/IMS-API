using IMS.Entities;
using IMS.Entities.Interfaces;
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
        public MailService(INotificationProvider notificationProvider, IEmployeeService employeeService)
        {
            this._notificationProvider = notificationProvider;
            this._employeeService = employeeService;
        }
        public async Task<bool> SendOrderRecieptToEmployee(EmployeeOrder employeeOrder)
        {
            try
            {
                var email = new Email();
                var employeeResponse = await _employeeService.ValidateEmployee(employeeOrder.Employee.Id);
                if (employeeResponse.Status.Equals(Status.Success))
                {
                    email.ToAddress = employeeResponse.Employee.Email;
                    email.Body = GenerateHTMLTemplateForEmployeeOrder(employeeOrder.EmployeeOrderDetails.EmployeeItemsQuantityList);
                    email.Subject = "Order Reciept";
                    return await _notificationProvider.SendEmail(email);
                }
                return false;
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }

        private string GenerateHTMLTemplateForEmployeeOrder(List<ItemQuantityMapping> employeeItemsQuantityList)
        {
            string emailBody = "You have taken &nbsp"+employeeItemsQuantityList.Count+"&nbsp Items<br><br>";
            emailBody += "<table>";
            emailBody+= "<tr><th style='float:left;'>Item Name</th><th>Quantity</th></tr>";
            foreach(var employeeItemQuantity in employeeItemsQuantityList)
            {
                emailBody += ("<tr><td>" + employeeItemQuantity.Item.Name + "</td>");
                emailBody+= ("<td>" + employeeItemQuantity.Quantity + "</td></tr>");
            }
            emailBody += "</table>";
            emailBody += "<br>If it's not you please report back to <a>mnaukarkar@tavisca.com</a>";
            return emailBody;
        }
    }
}