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
        
        public static List<Contracts.ItemQuantityMapping> ToDataContractsObject(List<Entities.ItemQuantityMapping> doItemQuantityMappings)
        {
            var dtoItemQuantityMappingList = new List<Contracts.ItemQuantityMapping>();
            if (doItemQuantityMappings != null)
            {
                foreach (var itemQuantityMapping in doItemQuantityMappings)
                {
                    IMS.Contracts.ItemQuantityMapping doitemQuantityMapping = new IMS.Contracts.ItemQuantityMapping()
                    {
                        Item = ToDataContractsObject(itemQuantityMapping.Item),
                        Quantity = itemQuantityMapping.Quantity
                    };
                    dtoItemQuantityMappingList.Add(doitemQuantityMapping);
                }
            }
            return dtoItemQuantityMappingList;
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
            }
            return dtoItem;
        }
    }
}
