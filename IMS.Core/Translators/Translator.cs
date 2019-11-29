
using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public static class Translator
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
                employeeValidationResponse.Error = ToDataContractsObject(entityresponse.Error);
            }
            return employeeValidationResponse;
        }

        private static Contracts.Employee ToDataContractsObject(Entities.Employee employee)
        {
            return new Contracts.Employee()
            {
                Id = employee.Id,
                Firstname = employee.Firstname,
                Lastname = employee.Lastname,
                Email = employee.Email,
                ContactNumber = null,
                TemporaryCardNumber = null,
                AccessCardNumber = null,
                IsActive = employee.IsActive
            };
        }

        public static Contracts.LoginResponse ToDataContractsObject(Entities.LoginResponse entityLoginResponse)
        {
            Contracts.LoginResponse loginResponse = new Contracts.LoginResponse();
            if (entityLoginResponse.Status == Entities.Status.Success)
            {
                loginResponse.Status = Contracts.Status.Success;
                loginResponse.AccessToken = entityLoginResponse.AccessToken;
                loginResponse.User = ToDataContractsObject(entityLoginResponse.User);
            }
            else
            {
                loginResponse.Status = Contracts.Status.Failure;
                loginResponse.Error = ToDataContractsObject(entityLoginResponse.Error); 
            }
            return loginResponse;

        }


        public static Contracts.ShelfResponse ToDataContractsObject(Entities.ShelfResponse entityShelfResponse)
        {
            Contracts.ShelfResponse shelfResponse = new Contracts.ShelfResponse();
            if (entityShelfResponse.Status == Entities.Status.Success)
            {
                shelfResponse.Status = Contracts.Status.Success;
                shelfResponse.Shelves = ToDataContractsObject(entityShelfResponse.Shelves);
            }
            else
            {
               shelfResponse.Status = Contracts.Status.Failure;
               shelfResponse.Error = ToDataContractsObject(entityShelfResponse.Error);
            }
            return shelfResponse;

        }

        public static Entities.Shelf ToEntitiesObject(Contracts.Shelf shelf)
        {
            return new Contracts.Shelf()
            {
                Id = shelf.Id,
                Name = shelf.Name,
                isActive = shelf.isActive

            };
        }

        private static List<Contracts.Shelf> ToDataContractsObject(List<Entities.Shelf> shelfList)
        {
            List<Contracts.Shelf> contractsShelfList = new List<Contracts.Shelf>();
            foreach (var entityList in shelfList)
            {
                Contracts.Shelf shelf = new Contracts.Shelf();
                shelf.Id = entityList.Id;
                shelf.Name = entityList.Name;
                shelf.Code = entityList.Code;
                contractsShelfList.Add(shelf);

            }
            return contractsShelfList;

        }


            public static Contracts.Error ToDataContractsObject(Entities.Error error)
        {
            return new Contracts.Error()
            {
                ErrorCode = error.ErrorCode,
                ErrorMessage = error.ErrorMessage
            };
        }

        public static Contracts.User ToDataContractsObject(Entities.User user)
        {
            return new Contracts.User()
            {
                Id = user.Id,
                Username = user.Username,
                Password = null,
                Firstname = user.Firstname,
                Lastname = user.Lastname,
                Email = user.Email,
                Role = ToDataContractsObject(user.Role)

            };
        }

        public static Contracts.Role ToDataContractsObject(Entities.Role role)
        {
            return new Contracts.Role()
                       {
                           Id = role.Id,
                            Name = role.Name
                       };
        }
        public static Entities.LoginRequest ToEntitiesObject(Contracts.LoginRequest contractsLoginRequest)
        {
            return new Entities.LoginRequest()
            {
                Username = contractsLoginRequest.Username,
                Password = contractsLoginRequest.Password

            };
        }


     }
}
