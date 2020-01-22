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
            if (doEmployee != null)
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
            return null;
        }
        public static Entities.Employee ToEntitiesObject(Contracts.Employee employee)
        {
            if (employee != null)
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
            return null; 
        }

        public static Contracts.EmployeeResponse ToDataContractsObject(Entities.EmployeeResponse entityEmployeeResponse)
        {
            Contracts.EmployeeResponse employeeResponseContract = new Contracts.EmployeeResponse();
            if (entityEmployeeResponse.Status == Entities.Status.Success)
            {
                employeeResponseContract.Status = Contracts.Status.Success;
                employeeResponseContract.Employees = entityEmployeeResponse.Employees == null ? null: ToDataContractsObject(entityEmployeeResponse.Employees);
                employeeResponseContract.PagingInfo = entityEmployeeResponse.PagingInfo == null ? null : Translator.ToDataContractsObject(entityEmployeeResponse.PagingInfo);
            }
            else
            {
                employeeResponseContract.Status = Contracts.Status.Failure;
                employeeResponseContract.Error = Translator.ToDataContractsObject(entityEmployeeResponse.Error);
            }
            return employeeResponseContract;
        }

        public static List<Contracts.Employee> ToDataContractsObject(List<Entities.Employee> employeesEntity)
        {
            List<Contracts.Employee> employeesContract = new List<Contracts.Employee>();
            if(employeesEntity!=null && employeesEntity.Count!=0)
            {
                foreach(Entities.Employee employeeEntity in employeesEntity)
                {
                    employeesContract.Add(ToDataContractsObject(employeeEntity));
                }
            }
            return employeesContract;
        }
    }
}
