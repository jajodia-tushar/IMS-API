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
            Entities.TransferToShelvesRequest contractTransferToShelvesRequest = new Entities.TransferToShelvesRequest();
            contractTransferToShelvesRequest.ShelvesItemsQuantityList = new List<Entities.TransferToShelfRequest>();
            foreach(Contracts.TransferToShelfRequest transferToShelfRequest in transferToShelvesRequest.ShelvesItemsQuantityList)
            {
                contractTransferToShelvesRequest.ShelvesItemsQuantityList.Add(ToEntitiesObject(transferToShelfRequest));
            }
            return contractTransferToShelvesRequest;
        }

        private static Entities.TransferToShelfRequest ToEntitiesObject(Contracts.TransferToShelfRequest transferToShelfRequest)
        {
            return new Entities.TransferToShelfRequest()
            {
                Shelf = Translator.ToEntitiesObject(transferToShelfRequest.Shelf),
                ItemQuantityMapping = Translator.ToEntitiesObject(transferToShelfRequest.ItemQuantityMapping)
            };
        }
    }
}
