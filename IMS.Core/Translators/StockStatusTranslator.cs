using System;
using System.Collections.Generic;
using System.Text;
using IMS.Contracts;
using IMS.Entities;

namespace IMS.Core.Translators
{
    public class StockStatusTranslator
    {
        public static Contracts.StockStatusResponse ToDataContractsObject(Entities.StockStatusResponse entityStockStatusResponse)
        {
            Contracts.StockStatusResponse contractStockStatusResponse = new Contracts.StockStatusResponse();
            if(entityStockStatusResponse.Status==Entities.Status.Success)
            {
                contractStockStatusResponse.Status = Contracts.Status.Success;
                contractStockStatusResponse.PagingInfo = new Contracts.PagingInfo()
                {
                    PageNumber = entityStockStatusResponse.PagingInfo.PageNumber,
                    PageSize = entityStockStatusResponse.PagingInfo.PageSize,
                    TotalResults = entityStockStatusResponse.PagingInfo.TotalResults
                };
                contractStockStatusResponse.StockStatusList = ToDataContractsObject(entityStockStatusResponse.StockStatusList);
            }
            else
            {
                contractStockStatusResponse.Status = Contracts.Status.Failure;
                contractStockStatusResponse.Error = Translator.ToDataContractsObject(entityStockStatusResponse.Error);
            }
            return contractStockStatusResponse;
        }

        public static List<Contracts.ItemStockStatus> ToDataContractsObject(List<Entities.ItemStockStatus> stockStatus)
        {
            List<Contracts.ItemStockStatus> contractStockStatusList = new List<Contracts.ItemStockStatus>();
            if(stockStatus!=null && stockStatus.Count>0 )
            {
                foreach(Entities.ItemStockStatus stockStatusList in stockStatus )
                {
                    Contracts.ItemStockStatus contractStockStatus = new Contracts.ItemStockStatus();

                    if (stockStatusList.Item!=null)
                    {
                        contractStockStatus.Item = ItemTranslator.ToDataContractsObject(stockStatusList.Item);
                    }
                    if(stockStatusList.StoreStatus!=null && stockStatusList.StoreStatus.Count>0)
                    {
                        contractStockStatus.StoreStatus = ToDataContractsObject(stockStatusList.StoreStatus);
                    }
                    contractStockStatusList.Add(contractStockStatus);
                }
            }
            return contractStockStatusList;
        }

        public static List<Contracts.StockStatus> ToDataContractsObject(List<Entities.StockStatus> list)
        {
            List<Contracts.StockStatus> contractList = new List<Contracts.StockStatus>();
            foreach(Entities.StockStatus itemStockStatus in list)
            {
                if(itemStockStatus!=null)
                {
                    contractList.Add(ToDataContractsObject(itemStockStatus));
                }
            }
            return contractList;
        }

        public static Contracts.StockStatus ToDataContractsObject(Entities.StockStatus storeColourQuantity)
        {
            return new Contracts.StockStatus()
            {
                Colour=ToDataContractsObject(storeColourQuantity.Colour),
                Location=storeColourQuantity.Location,
                Quantity=storeColourQuantity.Quantity
            };
        }
        public static Contracts.Colour ToDataContractsObject(Entities.Colour colour)
        {
            try
            {
                switch (colour)
                {
                    case Entities.Colour.Red:
                        return Contracts.Colour.Red;
                    case Entities.Colour.Amber:
                        return Contracts.Colour.Amber;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
            return Contracts.Colour.Green;
        }
    }
}
