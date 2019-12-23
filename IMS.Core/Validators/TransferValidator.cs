using IMS.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Validators
{
    public class TransferValidator
    {
        public static bool ValidateTransferToShelvesRequest(TransferToShelvesRequest transferToShelvesRequest)
        {
            if (transferToShelvesRequest == null)
            {
                return false;
            }
            if(transferToShelvesRequest.ShelvesItemsQuantityList==null || transferToShelvesRequest.ShelvesItemsQuantityList.Count<=0)
            {
                return false;
            }
            foreach (TransferToShelfRequest transferToShelfRequest in transferToShelvesRequest.ShelvesItemsQuantityList)
            {
                if (transferToShelfRequest == null || transferToShelfRequest.Shelf == null || transferToShelfRequest.ItemQuantityMapping == null)
                {
                    return false;
                }
                if (transferToShelfRequest.Shelf.Id == 0)
                {
                    return false;
                }
                foreach (ItemQuantityMapping itemQuantityMapping in transferToShelfRequest.ItemQuantityMapping)
                {
                    if (itemQuantityMapping.Item == null || itemQuantityMapping.Item.Id == 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
