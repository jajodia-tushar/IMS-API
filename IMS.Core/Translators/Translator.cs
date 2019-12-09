
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

        public static Contracts.VendorResponse ToDataContractsObject(Entities.VendorResponse getVendorResponseEntity)
        {
            Contracts.VendorResponse getVendorResponseContract = new Contracts.VendorResponse();
            if (getVendorResponseEntity.Status == Entities.Status.Success)
            {
                getVendorResponseContract.Status = Contracts.Status.Success;
                getVendorResponseContract.Vendors = ToDataContractsObject(getVendorResponseEntity.Vendors);
            }
            else
            {
                getVendorResponseContract.Status = Contracts.Status.Failure;
                getVendorResponseContract.Error = ToDataContractsObject(getVendorResponseEntity.Error);
            }
            return getVendorResponseContract;
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
        private static List<Contracts.Vendor> ToDataContractsObject(List<Entities.Vendor> vendors)
        {
            List<Contracts.Vendor> vendorsContract = new List<Contracts.Vendor>();
            foreach (Entities.Vendor vendorInstance in vendors)
            {
                vendorsContract.Add(ToDataContractsObject(vendorInstance));
            }
            return vendorsContract;
        }
        private static Contracts.Vendor ToDataContractsObject(Entities.Vendor vendor)
        {
            return new Contracts.Vendor()
            {
                Id = vendor.Id,
                Name = vendor.Name,
                ContactNumber = vendor.ContactNumber,
                PAN = vendor.PAN,
                CompanyIdentificationNumber = vendor.CompanyIdentificationNumber,
                GST = vendor.GST,
                Address = vendor.Address
            };
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
            return new Entities.Shelf()
            {
                Id = shelf.Id,
                Name = shelf.Name,
                IsActive = shelf.IsActive,
                Code = shelf.Code

            };
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
