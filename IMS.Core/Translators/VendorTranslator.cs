using System;
using System.Collections.Generic;
using System.Text;

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
                Title = entityVendor.Title,
                Address = entityVendor.Address
            };
        }
    }
}
