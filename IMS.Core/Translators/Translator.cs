using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public static class Translator
    {
        public static Contracts.Response ToDataContractsObject(Entities.Response doEntityTransferResponse)
        {
            Contracts.Response dtoContractTransferResponse = new Contracts.Response();
            if (doEntityTransferResponse.Status == Entities.Status.Success)
            {
                dtoContractTransferResponse.Status = Contracts.Status.Success;
            }
            else
            {
                dtoContractTransferResponse.Status = Contracts.Status.Failure;
                dtoContractTransferResponse.Error = ToDataContractsObject(doEntityTransferResponse.Error);
            }
            return dtoContractTransferResponse;
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

        internal static List<Entities.ItemQuantityMapping> ToEntitiesObject(List<Contracts.ItemQuantityMapping> contractItemQuantityMapping)
        {
            var entityItemQuantityMapping = new List<Entities.ItemQuantityMapping>();
            foreach (var itemQuantityMapping in contractItemQuantityMapping)
            {
                IMS.Entities.ItemQuantityMapping doitemQuantityMapping = new IMS.Entities.ItemQuantityMapping()
                {
                    Item = new IMS.Entities.Item()
                    {
                        Id = itemQuantityMapping.Item.Id,
                        Name = itemQuantityMapping.Item.Name,
                        MaxLimit = itemQuantityMapping.Item.MaxLimit,
                        IsActive = itemQuantityMapping.Item.IsActive
                    },
                    Quantity = itemQuantityMapping.Quantity
                };
                entityItemQuantityMapping.Add(doitemQuantityMapping);
            }
            return entityItemQuantityMapping;
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
        public static Contracts.Error ToDataContractsObject(Entities.Error error)
        {
            return new Contracts.Error()
            {
                ErrorCode = error.ErrorCode,
                ErrorMessage = error.ErrorMessage
            };
        }

        public static Entities.Item ToEntitiesObject(Contracts.Item contractsItem)
        {
            return new Entities.Item()
            {
                Id = contractsItem.Id,
                Name = contractsItem.Name,
                MaxLimit = contractsItem.MaxLimit,
                ImageUrl = contractsItem.ImageUrl,
                IsActive = contractsItem.IsActive,
                Rate = contractsItem.Rate
            };
        }
        public static Entities.Item ToEntitiesObject(Contracts.Item item)
        {
            return new Entities.Item()
            {
                Id = item.Id,
                Name = item.Name,
                IsActive = item.IsActive,
                MaxLimit = item.MaxLimit,
                Rate = item.Rate
            };
        }
    }
}
