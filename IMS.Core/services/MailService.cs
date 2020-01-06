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
        public MailService(INotificationProvider notificationProvider)
        {
            this._notificationProvider = notificationProvider;
        }
        public async Task<bool> SendOrderRecieptToEmployee(EmployeeOrder employeeOrder)
        {
            Email email = new Email();
            email.ToAddress = employeeOrder.Employee.Email;
            email.Body = GenerateHTMLTemplateForEmployeeOrder(employeeOrder.EmployeeOrderDetails.EmployeeItemsQuantityList);
            email.Subject = "Order Reciept";
            return await _notificationProvider.SendEmail(email);
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
