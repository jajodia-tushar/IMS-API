
using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public static class Translator
    {       
        public static Contracts.Response ToDataContractsObject(IMS.Entities.Response entitiesReponse)
        {
            return new Contracts.Response
            {
                Status = entitiesReponse.Status == Entities.Status.Success ? Contracts.Status.Success : Contracts.Status.Failure,
                Error = ToDataContractsObject(entitiesReponse.Error)
            };
        }
        public static Contracts.Error ToDataContractsObject(Entities.Error error)
        {

            if (error != null)
                return new Contracts.Error()
                {
                    ErrorCode = error.ErrorCode,
                    ErrorMessage = error.ErrorMessage
                };
            else
                return null;
        }
    }
}