using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public static class VendorTranslator
    {
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
                contractVendorResponse.Error = Translator.ToDataContractsObject(entityVendorResponse.Error);
            }
            return contractVendorResponse;
        }
        
        public static List<Contracts.Vendor> ToDataContractsObject(List<Entities.Vendor> entityVendors)
        {
            List<Contracts.Vendor> contractVendors = new List<Contracts.Vendor>();
            foreach (Entities.Vendor vendor in entityVendors)
            {
                contractVendors.Add(ToDataContractsObject(vendor));
            }
            return contractVendors;
        }

        public static Contracts.Vendor ToDataContractsObject(Entities.Vendor entityVendor)
        {
            return new Contracts.Vendor()
            {
                Id = entityVendor.Id,
                Name = entityVendor.Name,
                ContactNumber = entityVendor.ContactNumber,
                PAN = entityVendor.PAN,
                CompanyIdentificationNumber = entityVendor.CompanyIdentificationNumber,
                GST = entityVendor.GST,
                Title = entityVendor.Title,
                Address = entityVendor.Address
            };
        }
        public static Entities.Vendor ToEntitiesObject(Contracts.Vendor entityVendor)
        {
            if (entityVendor != null)
                return new Entities.Vendor()
                {
                    Id = entityVendor.Id,
                    Name = entityVendor.Name,
                    ContactNumber = entityVendor.ContactNumber,
                    PAN = entityVendor.PAN,
                    CompanyIdentificationNumber = entityVendor.CompanyIdentificationNumber,
                    GST = entityVendor.GST,
                    Title = entityVendor.Title,
                    Address = entityVendor.Address
                };
            return null;
        }

        public static Contracts.VendorsResponse ToDataContractsObject(Entities.VendorsResponse vendorResponseEntity)
        {
            Contracts.VendorsResponse vendorSearchResponse = new Contracts.VendorsResponse();
            vendorSearchResponse.PagingInfo = new Contracts.PagingInfo();
            vendorSearchResponse.Vendors = new List<Contracts.Vendor>();
            if (vendorResponseEntity!=null)
            {
                if (vendorResponseEntity.Status == Entities.Status.Success)
                {
                    vendorSearchResponse.Status = Contracts.Status.Success;
                    if(vendorResponseEntity.PagingInfo!=null)
                    {
                        vendorSearchResponse.PagingInfo = Translator.ToDataContractsObject(vendorResponseEntity.PagingInfo);
                    }
                    vendorSearchResponse.Vendors = ToDataContractsObject(vendorResponseEntity.Vendors);
                }
                else
                {
                    vendorSearchResponse.Status = Contracts.Status.Failure;
                    vendorSearchResponse.Error = Translator.ToDataContractsObject(vendorResponseEntity.Error);
                }
            }
            return vendorSearchResponse;
        }
    }
}
