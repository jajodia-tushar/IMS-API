using System;
using IMS.Contracts;
using IMS.Entities;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Translators
{
    public class ReportsTranslator
    {
        public static Contracts.MostConsumedItemsResponse ToDataContractsObject(Entities.MostConsumedItemsResponse dtoMostConsumedItemsResponse)
        {
            var doShelfItemsResponse = new Contracts.MostConsumedItemsResponse();
            if(dtoMostConsumedItemsResponse.Status == Entities.Status.Success)
            {
                doShelfItemsResponse.Status = Contracts.Status.Success;
                doShelfItemsResponse.ItemQuantityMapping = ToDataContractsObject(dtoMostConsumedItemsResponse.ItemQuantityMapping);
            }
            else
            {
                doShelfItemsResponse.Status = Contracts.Status.Failure;
                doShelfItemsResponse.Error = Translator.ToDataContractsObject(dtoMostConsumedItemsResponse.Error);
            }
            return doShelfItemsResponse;
        }
        
        private static List<Contracts.ItemQuantityMapping> ToDataContractsObject(List<Entities.ItemQuantityMapping> doItemQuantityMappings)
        {
            var dtoItemQuantityMappingList = new List<Contracts.ItemQuantityMapping>();
            foreach (var itemQuantityMapping in doItemQuantityMappings)
            {
                IMS.Contracts.ItemQuantityMapping doitemQuantityMapping = new IMS.Contracts.ItemQuantityMapping()
                {
                    Item = new IMS.Contracts.Item()
                    {
                        Id = itemQuantityMapping.Item.Id,
                        Name = itemQuantityMapping.Item.Name,
                        MaxLimit = itemQuantityMapping.Item.MaxLimit,
                        IsActive = itemQuantityMapping.Item.IsActive,
                        ImageUrl = itemQuantityMapping.Item.ImageUrl,
                        Rate = itemQuantityMapping.Item.Rate
                    },
                    Quantity = itemQuantityMapping.Quantity
                };
                dtoItemQuantityMappingList.Add(doitemQuantityMapping);
            }
            return dtoItemQuantityMappingList;
        }
    }
}
