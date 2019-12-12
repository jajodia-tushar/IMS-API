using System;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Translators
{
    public static class ItemTranslator
    {
        public static Contracts.ItemResponse ToDataContractsObject(Entities.ItemResponse entityItemResponse)
        {
            Contracts.ItemResponse itemResponse = new Contracts.ItemResponse();
            if (entityItemResponse.Status == Entities.Status.Success)
            {
                itemResponse.Status = Contracts.Status.Success;
                itemResponse.Items = ToDataContractsObject(entityItemResponse.Items);
            }
            else
            {
                itemResponse.Status = Contracts.Status.Failure;
                itemResponse.Error = ToDataContractsObject(entityItemResponse.Error);
            }
            return itemResponse;

        }
        public static List<Contracts.Item> ToDataContractsObject(List<Entities.Item> doEntityItemResponseList)
        {
            var dtoContractsItemList = new List<Contracts.Item>();
            foreach (var entityItem in doEntityItemResponseList)
            {
                Contracts.Item item = new Contracts.Item();
                item.Id = entityItem.Id;
                item.Name = entityItem.Name;
                item.MaxLimit = entityItem.MaxLimit;
                item.IsActive = entityItem.IsActive;
                item.ImageUrl = entityItem.ImageUrl;
                item.Rate = entityItem.Rate;
                dtoContractsItemList.Add(item);
            }
            return dtoContractsItemList;
        }
        public static Entities.Item ToEntitiesObject(Contracts.Item contractsItemRequest)
        {
            return new Entities.Item()
            {
                Id = contractsItemRequest.Id,
                Name = contractsItemRequest.Name,
                MaxLimit = contractsItemRequest.MaxLimit,
                IsActive = contractsItemRequest.IsActive,
                ImageUrl = contractsItemRequest.ImageUrl,
                Rate = contractsItemRequest.Rate
            };
        }

        public static Entities.ItemRequest ToEntitiesObject(Contracts.ItemRequest dtoItemRequest)
        {
            return new Entities.ItemRequest()
            {
                item = ToEntitiesObject(dtoItemRequest.item),
                ShelfMinimumLimit = ToEntitiesObject(dtoItemRequest.ShelfMinimumLimit),
                WarehouseMinimumLimit = dtoItemRequest.WarehouseMinimumLimit
            };
        }

        private static List<Entities.ShelfMinimumLimitMapping> ToEntitiesObject(List<Contracts.ShelfMinimumLimitMapping> dtoShelfMinimumLimit)
        {
            List<Entities.ShelfMinimumLimitMapping> shelfMinimumLimitMappings = new List<Entities.ShelfMinimumLimitMapping>();
            foreach (var shelfMinimum in dtoShelfMinimumLimit)
            {
                shelfMinimumLimitMappings.Add(new Entities.ShelfMinimumLimitMapping()
                {
                    ShelfId = shelfMinimum.ShelfId,
                    ShelfMinimumLimt = shelfMinimum.ShelfMinimumLimit
                });
            }
            return shelfMinimumLimitMappings;
        }

        public static Contracts.Error ToDataContractsObject(Entities.Error error)
        {
            return new Contracts.Error()
            {
                ErrorCode = error.ErrorCode,
                ErrorMessage = error.ErrorMessage
            };
        }
    }
}
