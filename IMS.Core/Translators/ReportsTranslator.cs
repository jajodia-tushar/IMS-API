﻿using System;
using IMS.Contracts;
using IMS.Entities;
using System.Collections.Generic;
using System.Text;

namespace IMS.Core.Translators
{
    public class ReportsTranslator
    {
        public static Contracts.ShelfWiseOrderCountResponse ToDataContractsObject(Entities.ShelfWiseOrderCountResponse doShelfWiseOrderCountResponse)
        {
            Contracts.ShelfWiseOrderCountResponse dtoShelfWiseOrderCountResponse =
                new Contracts.ShelfWiseOrderCountResponse();
            if(doShelfWiseOrderCountResponse.Status ==Entities.Status.Success)
            {
                dtoShelfWiseOrderCountResponse.Status = Contracts.Status.Success;
                dtoShelfWiseOrderCountResponse.DateWiseShelfOrderCount =
               ToDataContractsObject(doShelfWiseOrderCountResponse.DateWiseShelfOrderCount);
            }
            else
            {
                dtoShelfWiseOrderCountResponse.Status = Contracts.Status.Failure;
                dtoShelfWiseOrderCountResponse.Error = Translator.ToDataContractsObject(doShelfWiseOrderCountResponse.Error);
            }
            return dtoShelfWiseOrderCountResponse;
        }

        private static List<Contracts.ShelfOrderStats> ToDataContractsObject(List<Entities.ShelfOrderStats> doDateWiseShelfOrderCount)
        {
            List<Contracts.ShelfOrderStats> dtoDateShelfOrderMappings = new List<Contracts.ShelfOrderStats>();
            foreach (var dateShelfOrder in doDateWiseShelfOrderCount )
            {
               dtoDateShelfOrderMappings.Add(ToDataContractsObject(dateShelfOrder));
            }

            return dtoDateShelfOrderMappings;
        }

        private static Contracts.ShelfOrderStats ToDataContractsObject(Entities.ShelfOrderStats doDateShelfOrder)
        {
            Contracts.ShelfOrderStats dtoDateShelfOrderMapping = new Contracts.ShelfOrderStats();
            dtoDateShelfOrderMapping.Date = doDateShelfOrder.Date;
            dtoDateShelfOrderMapping.ShelfOrderCountMappings = ToDataContractsObject(doDateShelfOrder.ShelfOrderCountMappings);
            return dtoDateShelfOrderMapping;
        }

        private static List<Contracts.ShelfOrderCountMapping> ToDataContractsObject(List<Entities.ShelfOrderCountMapping> doShelfOrderCountMappings)
        {
            List<Contracts.ShelfOrderCountMapping> dtoShelfOrderCountMappings = new List<Contracts.ShelfOrderCountMapping>();
            foreach(var shelfOrder in doShelfOrderCountMappings)
            {
                dtoShelfOrderCountMappings.Add(ToDataContractsObject(shelfOrder));
            }
            return dtoShelfOrderCountMappings;
        }

        private static Contracts.ShelfOrderCountMapping ToDataContractsObject(Entities.ShelfOrderCountMapping doShelfOrder)
        {
            Contracts.ShelfOrderCountMapping dtoShelfOrder = new Contracts.ShelfOrderCountMapping();
            dtoShelfOrder.ShelfName = doShelfOrder.ShelfName;
            dtoShelfOrder.OrderCount = doShelfOrder.OrderCount;
            return dtoShelfOrder;
        }
    }
}
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

        public static Contracts.ItemsConsumptionReport ToDataContractsObject(Entities.ItemsConsumptionReport itemConsumptionReportEntity)
        {
            var contractItemConsumptionReport = new Contracts.ItemsConsumptionReport();
            if (itemConsumptionReportEntity.Status == Entities.Status.Success)
            {
                contractItemConsumptionReport.Status = Contracts.Status.Success;
                contractItemConsumptionReport.ItemConsumptions = ToDataContractsObject(itemConsumptionReportEntity.ItemConsumptions);
            }
            else
            {
                contractItemConsumptionReport.Status = Contracts.Status.Failure;
                contractItemConsumptionReport.Error = Translator.ToDataContractsObject(itemConsumptionReportEntity.Error);
            }
            return contractItemConsumptionReport;
        }

        public static List<Contracts.DateItemConsumption> ToDataContractsObject(List<Entities.DateItemConsumption> itemConsumptionsEntity)
        {
            List<Contracts.DateItemConsumption> contractDateItemConsumptions = new List<Contracts.DateItemConsumption>();
            if(itemConsumptionsEntity !=null && itemConsumptionsEntity.Count!=0)
            {
                foreach(Entities.DateItemConsumption dateItemConsumption in itemConsumptionsEntity)
                {
                    if(dateItemConsumption!=null)
                    {
                        contractDateItemConsumptions.Add(ToDataContractsObject(dateItemConsumption));
                    }
                }
            }
            return contractDateItemConsumptions;
        }
        public static Contracts.DateItemConsumption ToDataContractsObject(Entities.DateItemConsumption dateItemConsumptionEntity)
        {
            return new Contracts.DateItemConsumption()
            {
                Date = dateItemConsumptionEntity.Date,
                ItemsConsumptionCount = dateItemConsumptionEntity.ItemsConsumptionCount
            };
        }
    }
}
