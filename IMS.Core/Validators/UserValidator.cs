    
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

        public static bool ValidateNewUser(User user)
        {
            if (string.IsNullOrEmpty(user.Firstname.Trim()) || string.IsNullOrEmpty(user.Username.Trim()) || string.IsNullOrEmpty(user.Password.Trim()) || string.IsNullOrEmpty(user.Firstname.Trim()) || user.Role == null || user.Role.Id <= 0)
                return false;
            CheckUserNameFormat(user.Username);
            CheckEmailFormat(user.Email);
            CheckPasswordFormat(user.Password);
            return true;
        }


        public static void CheckUserNameFormat(string userName)
        {
            if (userName.Length < 6 || userName.Length > 15)
                throw new InvalidUserNameException(Constants.ErrorMessages.UserNameMinMaxLengthError);
            if (userName.Contains(" "))
                throw new InvalidUserNameException(Constants.ErrorMessages.UserNameSpacesError);
            return;
        }

        public static void CheckPasswordFormat(string password)
        {
            Regex hasNumber = new Regex(@"[0-9]+");
            Regex hasUpperChar = new Regex(@"[A-Z]+");
            Regex hasMiniMaxChars = new Regex(@".{8,15}");
            Regex hasLowerChar = new Regex(@"[a-z]+");
            Regex hasSymbols = new Regex(@"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
            if (password.Contains(" "))
            {
                throw new InvalidPasswordException(Constants.ErrorMessages.PasswordSpaceError);
            }
            else if (!hasLowerChar.IsMatch(password))
            {
                throw new InvalidPasswordException(Constants.ErrorMessages.PasswordLowerCaseError);
            }
            else if (!hasUpperChar.IsMatch(password))
            {
                throw new InvalidPasswordException(Constants.ErrorMessages.PasswordUpperCaseError);
            }
            else if (!hasMiniMaxChars.IsMatch(password))
            {
                throw new InvalidPasswordException(Constants.ErrorMessages.PasswordLengthError);
            }
            else if (!hasNumber.IsMatch(password))
            {
                throw new InvalidPasswordException(Constants.ErrorMessages.PasswordNumberError);
            }

            else if (!hasSymbols.IsMatch(password))
            {
                throw new InvalidPasswordException(Constants.ErrorMessages.PasswordSymbolError);
            }
            else
            {
                return;
            }
        }


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
        public static bool UpdateUserValidation(User user)
        {
            if (user.Id <= 0 || String.IsNullOrEmpty(user.Firstname) || String.IsNullOrEmpty(user.Lastname) || String.IsNullOrEmpty(user.Email) || user.Role.Id == 0)
            {
                return false;
            }
            CheckEmailFormat(user.Email);
            return true;
        }





    }
}
 










