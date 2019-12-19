using System;
using IMS.Contracts;
using IMS.Entities;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Translators
{
    public class ReportsTranslator
    {
        public static Contracts.MostConsumedItemsResponse ToDataContractsObject(Entities.MostConsumedItemsResponse doMostConsumedItemsResponse)
        {
            var dtoMostConsumedItemsResponse = new Contracts.MostConsumedItemsResponse();
            if(doMostConsumedItemsResponse.Status == Entities.Status.Success)
            {
                dtoMostConsumedItemsResponse.Status = Contracts.Status.Success;
                dtoMostConsumedItemsResponse.ItemQuantityMapping = ToDataContractsObject(doMostConsumedItemsResponse.ItemQuantityMapping);
            }
            else
            {
                dtoMostConsumedItemsResponse.Status = Contracts.Status.Failure;
                dtoMostConsumedItemsResponse.Error = Translator.ToDataContractsObject(doMostConsumedItemsResponse.Error);
            }
            return dtoMostConsumedItemsResponse;
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
