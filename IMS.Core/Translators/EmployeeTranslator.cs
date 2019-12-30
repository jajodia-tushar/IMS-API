using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public static class EmployeeTranslator
    {
        public static Contracts.GetEmployeeResponse ToDataContractsObject(Entities.GetEmployeeResponse entityresponse)
        {
            Contracts.GetEmployeeResponse employeeValidationResponse = new Contracts.GetEmployeeResponse();
            if (entityresponse.Status == Entities.Status.Success)
            {
                employeeValidationResponse.Status = Contracts.Status.Success;
                employeeValidationResponse.Employee = ToDataContractsObject(entityresponse.Employee);
            }
            else
            {
                employeeValidationResponse.Status = Contracts.Status.Failure;
                employeeValidationResponse.Error = Translator.ToDataContractsObject(entityresponse.Error);
            }
            return employeeValidationResponse;
        }
        public static Contracts.Employee ToDataContractsObject(Entities.Employee doEmployee)
        {
            return new Contracts.Employee()
            {
                Id = doEmployee.Id,
                Firstname = doEmployee.Firstname,
                Lastname = doEmployee.Lastname,
                Email = doEmployee.Email,
                ContactNumber = doEmployee.ContactNumber,
                TemporaryCardNumber = doEmployee.TemporaryCardNumber,
                AccessCardNumber = doEmployee.AccessCardNumber,
                IsActive = doEmployee.IsActive
            };
        }
        public static Entities.Employee ToEntitiesObject(Contracts.Employee employee)
        {
            return new Entities.Employee()
            {
                Id = employee.Id,
                Firstname = employee.Firstname,
                Lastname = employee.Lastname,
                Email = employee.Email,
                ContactNumber = employee.ContactNumber,
                TemporaryCardNumber = employee.TemporaryCardNumber,
                AccessCardNumber = employee.AccessCardNumber,
                IsActive = employee.IsActive
            };
        }
    }
}
