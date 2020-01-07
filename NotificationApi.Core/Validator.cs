using NotificationApi.Core.Models;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace NotificationApi.Core
{
    public class Validator
    {
        public static bool Validate(Email email)
        {
            bool isValidEmail = false;
            isValidEmail = ValidateEmailAddress(email.ToAddress);
            if (!string.IsNullOrEmpty(email.CC))
                isValidEmail = ValidateEmailAddress(email.CC);
            return isValidEmail;
        }
        public static bool ValidateEmailAddress(string emailAddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailAddress);



            }
            catch (FormatException)
            {
                return false;
            }

            return true;
        }
        
       
    }
}
