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
        public static Contracts.PagingInfo ToDataContractsObject(Entities.PagingInfo pagingInfo)
        {
            if (pagingInfo != null)
            {
                return new Contracts.PagingInfo
                {
                    PageNumber = pagingInfo.PageNumber,
                    PageSize = pagingInfo.PageSize,
                    TotalResults = pagingInfo.TotalResults
                };
            }
            return null;
        }
    }
}
