using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using NotificationApi.Core.Models;

namespace NotificationApi.Core
{
    public class EmailService : IEmailService
    {
        public async Task<Response> Send(Email email)
        {
            Response response = new Response
            {
                Status = Status.Failure
            };
            try
            {
                SmtpClient client = new SmtpClient("smtp.gmail.com");
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            System.Net.NetworkCredential credentials =
                new System.Net.NetworkCredential("taviscaims@gmail.com", "Tavisca@123");
            client.EnableSsl = true;
            client.Credentials = credentials;        
            var mail = new MailMessage("taviscaims@gmail.com", email.ToAddress);
            mail.Subject = email.Subject;
            mail.Body = email.Body;
                if (email.CC != null)
                {
                    MailAddress copy = new MailAddress(email.CC);
                    mail.CC.Add(copy);
                }
            mail.SubjectEncoding = System.Text.Encoding.UTF8;
            mail.IsBodyHtml = true;
            await client.SendMailAsync(mail);
            response.Status = Status.Success;
            }
            catch (Exception ex)
            {

                throw ex;
            }
          return response ;
        }
    }
}
