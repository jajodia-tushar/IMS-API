    
﻿using IMS.Entities;
using IMS.Entities.Exceptions;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;


namespace IMS.Core.Validators
{
    public class UserValidator
    {
       



       

        public static void CheckEmailFormat(string email)
        {
            try
            {
                MailAddress m = new MailAddress(email);



            }
            catch (FormatException)
            {
                throw new InvalidEmailException(Constants.ErrorMessages.EmailFormatError);
            }


        }


       


       
    }
}
 










