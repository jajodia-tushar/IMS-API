using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public class TransferTranslator
    {
        public static Entities.TransferToShelvesRequest ToEntitiesObject(Contracts.TransferToShelvesRequest transferToShelvesRequest)
        {
            return new Entities.TransferToShelvesRequest()
            {

            };
        }

        public static Contracts.Response ToDataContractsObject(Entities.Response entityTransferToShelfResponse)
        {
            return new Contracts.Response()
            {

            };
        }
    }
}
