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

        public static Contracts.Item ToDataContractsObject(Entities.Item item)
        {
            return new Contracts.Item()
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
