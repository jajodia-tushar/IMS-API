using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public class TransferTranslator
    {
        public static Entities.TransferToShelvesRequest ToEntitiesObject(Contracts.TransferToShelvesRequest transferRequest)
        {
            Entities.TransferToShelvesRequest contractTransferRequest = new Entities.TransferToShelvesRequest();
            contractTransferRequest.ShelvesItemsQuantityList = new List<Entities.TransferToShelfRequest>();
            if (transferRequest == null || transferRequest.ShelvesItemsQuantityList == null || transferRequest.ShelvesItemsQuantityList.Count > 0)
            {
                foreach (Contracts.TransferToShelfRequest transferRequestIterator in transferRequest.ShelvesItemsQuantityList)
                {

                    if (transferRequestIterator != null)
                    {
                        contractTransferRequest.ShelvesItemsQuantityList.Add(ToEntitiesObject(transferRequestIterator));
                    }
                }
            }
            return contractTransferRequest;
        }

        private static Entities.TransferToShelfRequest ToEntitiesObject(Contracts.TransferToShelfRequest transferToShelfRequest)
        {
            Entities.TransferToShelfRequest transferRequestEntity = new Entities.TransferToShelfRequest();
            if (transferToShelfRequest.ItemQuantityMapping != null && transferToShelfRequest.ItemQuantityMapping.Count > 0)
            {
                transferRequestEntity.ItemQuantityMapping = ShelfItemsTranslator.ToEntitiesObject(transferToShelfRequest.ItemQuantityMapping);
            }
            if (transferToShelfRequest.Shelf != null)
            {
                transferRequestEntity.Shelf = ShelfTranslator.ToEntitiesObject(transferToShelfRequest.Shelf);
            }
            return transferRequestEntity;
        }
    }
}
