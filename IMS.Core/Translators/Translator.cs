
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

        public static Contracts.VendorResponse ToDataContractsObject(Entities.VendorResponse entityVendorResponse)
        {
            Contracts.VendorResponse contractVendorResponse = new Contracts.VendorResponse();
            if (entityVendorResponse.Status == Entities.Status.Success)
            {
                contractVendorResponse.Status = Contracts.Status.Success;
                contractVendorResponse.Vendors = ToDataContractsObject(entityVendorResponse.Vendors);
            }
            else
            {
                contractVendorResponse.Status = Contracts.Status.Failure;
                contractVendorResponse.Error = ToDataContractsObject(entityVendorResponse.Error);
            }
            return contractVendorResponse;
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
        private static List<Contracts.Vendor> ToDataContractsObject(List<Entities.Vendor> entityVendors)
        {
            List<Contracts.Vendor> contractVendors = new List<Contracts.Vendor>();
            foreach (Entities.Vendor vendor in entityVendors)
            {
                contractVendors.Add(ToDataContractsObject(vendor));
            }
            return contractVendors;
        }
        private static Contracts.Vendor ToDataContractsObject(Entities.Vendor entityVendor)
        {
            return new Contracts.Vendor()
            {
                Id = entityVendor.Id,
                Name = entityVendor.Name,
                ContactNumber = entityVendor.ContactNumber,
                PAN = entityVendor.PAN,
                CompanyIdentificationNumber = entityVendor.CompanyIdentificationNumber,
                GST = entityVendor.GST,
                Address = entityVendor.Address
            };
        }


        public static Contracts.ShelfResponse ToDataContractsObject(Entities.ShelfResponse entityShelfResponse)
        {
            Contracts.ShelfItemsResponse shelfItemsResponseContract = new Contracts.ShelfItemsResponse();
            shelfItemsResponseContract.Status = Contracts.Status.Failure;
            if(shelfItemsResponseEntity.Error!=null)
                shelfItemsResponseContract.Error = ToDataContractsObject(shelfItemsResponseEntity.Error);
            if (shelfItemsResponseEntity.Status == Entities.Status.Success)
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
            return new Entities.Shelf()
            {
                Id = shelf.Id,
                Name = shelf.Name,
                IsActive = shelf.IsActive,
                Code = shelf.Code

            };
            IMS.Contracts.Shelf shelfContract = new Contracts.Shelf();
            shelfContract.Id = shelfEntity.Id;
            shelfContract.Name = shelfEntity.Name;
            shelfContract.isActive = shelfEntity.IsActive;
            shelfContract.Code = shelfContract.Code;
            return shelfContract;
        }
        
        private static List<Contracts.Shelf> ToDataContractsObject(List<Entities.Shelf> shelfs)
        {
            List<Contracts.Shelf> dtoShelves = new List<Contracts.Shelf>();
            foreach (var shelf in shelfs)
            {
                Contracts.Shelf dtoShelf = new Contracts.Shelf();
                dtoShelf.Id = shelf.Id;
                dtoShelf.Name = shelf.Name;
                dtoShelf.Code = shelf.Code;
                dtoShelf.IsActive = shelf.IsActive;
                dtoShelves.Add(dtoShelf);

            }
            return dtoShelves;

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
