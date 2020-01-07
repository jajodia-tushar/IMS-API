using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NotificationApi.Core.Models;

namespace NotificationApi.Core
{
    public class EmailService : IEmailService
    {
        private IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;

        }
        public async Task<Response> Send(Email email)
        {
            Response response = new Response
            {
                Status = Status.Failure
            };
            try
            {
                if (Validator.Validate(email))
                {
                    SmtpClient client = new SmtpClient(_configuration["SmtpServer:Host"]);
                    client.Port = int.Parse(_configuration["SmtpServer:Port"]);
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    System.Net.NetworkCredential credentials =
                        new System.Net.NetworkCredential(_configuration["Email:FromAddress"], _configuration["Email:Password"]);
                    client.EnableSsl = true;
                    client.Credentials = credentials;
                    var mail = new MailMessage(_configuration["Email:FromAddress"], email.ToAddress);
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
                else
                {
                    response.Error = new Error
                    {
                        ErrorCode = Constants.ErrorCodes.BadRequest,
                        ErrorMessage = Constants.ErrorMessages.InvalidEmail
                    };
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
          return response ;
        }
    }
}
