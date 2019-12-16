using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public static class ShelfItemsTranslator
    {
        public static Contracts.ShelfItemsResponse ToDataContractsObject(Entities.ShelfItemsResponse doShelfItemsResponse)
        {
            var dtoShelfItemsResponse = new Contracts.ShelfItemsResponse();
            dtoShelfItemsResponse.Status = Contracts.Status.Failure;
            if (doShelfItemsResponse.Error != null)
                dtoShelfItemsResponse.Error =Translator.ToDataContractsObject(doShelfItemsResponse.Error);
            if (doShelfItemsResponse.Status == Entities.Status.Success)
            {
                dtoShelfItemsResponse.Status = Contracts.Status.Success;
                dtoShelfItemsResponse.Error = null;
                dtoShelfItemsResponse.itemQuantityMappings = ToDataContractsObject(doShelfItemsResponse.ItemQuantityMappings);
            }
            if (doShelfItemsResponse.Shelf != null)
                dtoShelfItemsResponse.shelf = ShelfTranslator.ToDataContractsObject(doShelfItemsResponse.Shelf);
            return dtoShelfItemsResponse;
        }

        public static List<Contracts.ItemQuantityMapping> ToDataContractsObject(List<Entities.ItemQuantityMapping> doItemQuantityMappings)
        {
            var dtoItemQuantityMappingList = new List<Contracts.ItemQuantityMapping>();
            foreach (var itemQuantityMapping in doItemQuantityMappings)
            {
                IMS.Contracts.ItemQuantityMapping dtoitemQuantityMapping = new IMS.Contracts.ItemQuantityMapping()
                {
                    Item = new IMS.Contracts.Item()
                    {
                        Id = itemQuantityMapping.Item.Id,
                        Name = itemQuantityMapping.Item.Name,
                        MaxLimit = itemQuantityMapping.Item.MaxLimit,
                        IsActive = itemQuantityMapping.Item.IsActive
                    },
                    Quantity = itemQuantityMapping.Quantity
                };
                dtoItemQuantityMappingList.Add(dtoitemQuantityMapping);
            }
            return dtoItemQuantityMappingList;
        }

        public static List<Entities.ItemQuantityMapping> ToEntitiesObject(List<Contracts.ItemQuantityMapping> contractItemsQuantityList)
        {
            List<Entities.ItemQuantityMapping> entityItemQuantityList = new List<Entities.ItemQuantityMapping>();
            foreach (Contracts.ItemQuantityMapping itemQuantityMapping in contractItemsQuantityList)
            {
                IMS.Entities.ItemQuantityMapping entityItemQuantityMapping = new IMS.Entities.ItemQuantityMapping()
                {
                    Item = Translator.ToEntitiesObject(itemQuantityMapping.Item),
                    Quantity = itemQuantityMapping.Quantity
                };
                entityItemQuantityList.Add(entityItemQuantityMapping);
            }
            return entityItemQuantityList;
        }
    }
}
