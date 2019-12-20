using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Translators
{
    public static class ItemTranslator
    {
        public static Contracts.ItemResponse ToDataContractsObject(Entities.ItemResponse doItemResponse)
        {
            Contracts.ItemResponse dtoItemResponse = new Contracts.ItemResponse();
            if (doItemResponse.Status == Entities.Status.Success)
            {
                dtoItemResponse.Status = Contracts.Status.Success;
                dtoItemResponse.Items = ToDataContractsObject(doItemResponse.Items);
            }
            else
            {
                dtoItemResponse.Status = Contracts.Status.Failure;
                dtoItemResponse.Error = Translator.ToDataContractsObject(doItemResponse.Error);
            }
            return dtoItemResponse;

        }
        public static List<Contracts.Item> ToDataContractsObject(List<Entities.Item> doItemList)
        {
            var dtoItemList = new List<Contracts.Item>();
            if (doItemList != null)
            {
                foreach (var doItem in doItemList)
                {
                    Contracts.Item dtoItem = ToDataContractsObject(doItem);
                    dtoItemList.Add(dtoItem);
                }
            }
            return dtoItemList;
        }
        public static Contracts.Item ToDataContractsObject(Entities.Item doItem)
        {
            Contracts.Item dtoItem = new Contracts.Item();
            if (doItem != null)
            {
                dtoItem.Id = doItem.Id;
                dtoItem.Name = doItem.Name;
                dtoItem.MaxLimit = doItem.MaxLimit;
                dtoItem.IsActive = doItem.IsActive;
                dtoItem.ImageUrl = doItem.ImageUrl;
                dtoItem.Rate = doItem.Rate;
                dtoItem.ShelvesRedLimit = doItem.ShelvesRedLimit;
                dtoItem.ShelvesAmberLimit = doItem.ShelvesAmberLimit;
                dtoItem.WarehouseRedLimit = doItem.WarehouseRedLimit;
                dtoItem.WarehouseAmberLimit = doItem.WarehouseAmberLimit;
            }
            return dtoItem;
        }
        public static Entities.Item ToEntitiesObject(Contracts.Item dtoItemRequest)
        {
            Entities.Item doItem = new Entities.Item();
            if (doItem != null)
            {
                doItem.Id = dtoItemRequest.Id;
                doItem.Name = dtoItemRequest.Name;
                doItem.MaxLimit = dtoItemRequest.MaxLimit;
                doItem.IsActive = dtoItemRequest.IsActive;
                doItem.ImageUrl = dtoItemRequest.ImageUrl;
                doItem.Rate = dtoItemRequest.Rate;
                doItem.ShelvesRedLimit = dtoItemRequest.ShelvesRedLimit;
                doItem.ShelvesAmberLimit = dtoItemRequest.ShelvesAmberLimit;
                doItem.WarehouseRedLimit = dtoItemRequest.WarehouseRedLimit;
                doItem.WarehouseAmberLimit = dtoItemRequest.WarehouseAmberLimit;
            }
            return doItem;
        }
    }
}
