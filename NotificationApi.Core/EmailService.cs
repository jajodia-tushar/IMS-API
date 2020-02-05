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
                    using (var emailMessage = new MailMessage())
                    {
                            emailMessage.From = new MailAddress(email.FromAddress);
                            emailMessage.To.Add(new MailAddress(email.ToAddress));
                            emailMessage.Subject = email.Subject;
                            emailMessage.Body = email.Body;
                            emailMessage.IsBodyHtml = true;
                            if (email.CC != null)
                            {
                                MailAddress copy = new MailAddress(email.CC);
                                emailMessage.CC.Add(copy);
                            }
                            using (var client = new SmtpClient())
                            {
                                client.Host = _configuration["SmtpServer:Host"];
                                client.Port = int.Parse(_configuration["SmtpServer:Port"]); 
                                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                                client.Send(emailMessage);
                            }
                        response.Status = Status.Success;
                    }
                    
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
