using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace IMS.Core.Validators
{
    public class EmployeeValidator
    {
        public static EmployeeResponse EmployeeValidate(Employee employee)
        {
            EmployeeResponse employeeResponse = new EmployeeResponse();
            if (String.IsNullOrEmpty(employee.Id) || String.IsNullOrEmpty(employee.Email) || String.IsNullOrEmpty(employee.Firstname))
            {
                employeeResponse.Status = Status.Failure;
                employeeResponse.Error = Utility.ErrorGenerator(Constants.ErrorCodes.BadRequest, Constants.ErrorMessages.InvalidEmployeeDetails);
            }
            return employeeResponse;
        }
    }
}